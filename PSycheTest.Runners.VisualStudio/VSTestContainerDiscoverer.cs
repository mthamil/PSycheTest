using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestWindow.Extensibility;
using PSycheTest.Runners.Framework;
using PSycheTest.Runners.VisualStudio.Core;
using PSycheTest.Runners.VisualStudio.Core.Monitoring;
using PSycheTest.Runners.VisualStudio.Core.Utilities;
using PSycheTest.Runners.VisualStudio.Core.Utilities.InputOutput;
using ILogger = PSycheTest.Runners.Framework.ILogger;
using ValidateArg = Microsoft.VisualStudio.TestPlatform.ObjectModel.ValidateArg;

namespace PSycheTest.Runners.VisualStudio
{
	/// <summary>
	/// Finds PowerShell scripts containing tests.
	/// </summary>
	[Export(typeof(ITestContainerDiscoverer))]
	public class VSTestContainerDiscoverer : ITestContainerDiscoverer, IDisposable
	{
		/// <summary>
		/// Initializes a new <see cref="VSTestContainerDiscoverer"/>.
		/// </summary>
		/// <param name="serviceProvider">Provides access to Visual Studio services</param>
		/// <param name="solutionMonitor">Monitors a solution for changes</param>
		/// <param name="projectFileMonitor">Monitors a project for changes to its child items</param>
		[ImportingConstructor]
		public VSTestContainerDiscoverer(
			[Import(typeof(SVsServiceProvider))] IServiceProvider serviceProvider,
			ISolutionMonitor solutionMonitor,
			IProjectFileMonitor projectFileMonitor)
			: this(serviceProvider,
				   new ActivityLogger(serviceProvider.GetService<IVsActivityLog, SVsActivityLog>(), typeof(VSTestContainerDiscoverer)), 
			       solutionMonitor, 
			       projectFileMonitor,
				   new FileSystemWatcherAdapter(new FileSystemWatcher()),
				   (disc, file, project) => new PowerShellTestContainer(disc, file, project))
		{
		}

		/// <summary>
		/// Initializes a new <see cref="VSTestContainerDiscoverer"/>.
		/// </summary>
		/// <param name="serviceProvider">Provides access to Visual Studio services</param>
		/// <param name="logger">Object that logs messages</param>
		/// <param name="solutionMonitor">Monitors a solution for changes</param>
		/// <param name="projectFileMonitor">Monitors a project for changes to its child items</param>
		/// <param name="fileWatcher">A file system watcher</param>
		/// <param name="containerFactory">Creates new <see cref="ITestContainer"/>s</param>
		public VSTestContainerDiscoverer(
			IServiceProvider serviceProvider,
			ILogger logger,
			ISolutionMonitor solutionMonitor,
			IProjectFileMonitor projectFileMonitor,
			IFileSystemWatcher fileWatcher,
			Func<ITestContainerDiscoverer, string, IVsProject, IVSTestContainer> containerFactory)
		{
			_serviceProvider = ValidateArg.NotNull(serviceProvider, "serviceProvider");
			_logger = ValidateArg.NotNull(logger, "logger");
			_solutionMonitor = ValidateArg.NotNull(solutionMonitor, "solutionMonitor");
			_projectFileMonitor = ValidateArg.NotNull(projectFileMonitor, "projectFileMonitor");
			_fileWatcher = ValidateArg.NotNull(fileWatcher, "fileWatcher");
			_containerFactory = ValidateArg.NotNull(containerFactory, "containerFactory");

			_solutionMonitor.SolutionChanged += solutionMonitor_SolutionChanged;
			_solutionMonitor.ProjectChanged += solutionMonitor_ProjectChanged;
			_solutionMonitor.StartMonitoring();

			_projectFileMonitor.FileChanged += projectFileMonitor_FileChanged;
			_projectFileMonitor.StartMonitoring();

			_fileWatcher.NotifyFilter = NotifyFilters.LastWrite;
			_fileWatcher.Filter = String.Format("*{0}", TestScript.FileExtension);
			_fileWatcher.IncludeSubdirectories = true;
			_fileWatcher.Error += fileWatcher_Error;
		}

		/// <summary>
		/// Initializes cache with test containers from the current solution.
		/// </summary>
		private void Initialize()
		{
			var solution = _serviceProvider.GetService<IVsSolution, SVsSolution>();
			var projects = solution.Info().GetProjects().Where(p => p.Info().IsPhysicalProject);
			foreach (var project in projects)
				AddProjectToCache(project);

			_fileWatcher.Path = solution.Info().GetDirectory().FullName;
			_fileWatcher.Changed += fileWatcher_Changed;
			_fileWatcher.EnableRaisingEvents = true;
			_isInitialized = true;
		}

		void solutionMonitor_SolutionChanged(object sender, SolutionChangedEventArgs e)
		{
			switch (e.ChangeType)
			{
				case SolutionChangeType.Loaded:
					if (!_isInitialized)
						Initialize();
					OnTestContainersUpdated();
					break;

				case SolutionChangeType.Unloaded:
					_containerCache.Clear();

					_fileWatcher.EnableRaisingEvents = false;
					_fileWatcher.Changed -= fileWatcher_Changed;
					_isInitialized = false;
					break;
			}
		}

		void solutionMonitor_ProjectChanged(object sender, ProjectChangedEventArgs e)
		{
			switch (e.ChangeType)
			{
				case ProjectChangeType.Loaded:
					RemoveProjectFromCache(e.Project);

					var containers = DiscoverTestContainers(e.Project);
					foreach (var container in containers)
						AddContainerToCache(container, e.Project);

					break;

				case ProjectChangeType.Unloaded:
					RemoveProjectFromCache(e.Project);
					break;
			}

			OnTestContainersUpdated();
		}

		void projectFileMonitor_FileChanged(object sender, ProjectFileChangedEventArgs e)
		{
			switch (e.ChangeType)
			{
				case FileChangeType.Added:
					AddContainerToCache(_containerFactory(this, e.FilePath, e.Project), e.Project);
					break;

				case FileChangeType.Removed:
					RemoveContainerFromCache(e.FilePath, e.Project);
					break;
			}

			OnTestContainersUpdated();
		}

		void fileWatcher_Changed(object sender, FileSystemEventArgs e)
		{
			// This check prevents handling of the event multiple times in a row
			// since it seems to be raised at least twice for a single change.
			DateTime lastWriteTime = File.GetLastWriteTime(e.FullPath);
			if (lastWriteTime != _lastFileUpdate)
			{
				if (e.ChangeType == WatcherChangeTypes.Changed)
				{
					// Only refresh for files included in the solution.
					IVSTestContainer oldContainer;
					if (_containerCache.TryRemove(e.FullPath, out oldContainer))
					{
						// Create a new container.
						_containerCache[oldContainer.Source] = _containerFactory(this, oldContainer.Source, oldContainer.Project);
						OnTestContainersUpdated();
					}
				}

				_lastFileUpdate = lastWriteTime;
			}
		}

		void fileWatcher_Error(object sender, ErrorEventArgs e)
		{
			var exception = e.GetException();
			_logger.Error("FileSystemWatcher error: {0}{1}{2}", exception.Message, Environment.NewLine, exception.StackTrace);
		}

		/// <see cref="ITestContainerDiscoverer.ExecutorUri"/>
		public Uri ExecutorUri { get { return VSTestExecutor.ExecutorUri; } }

		/// <see cref="ITestContainerDiscoverer.TestContainers"/>
		public IEnumerable<ITestContainer> TestContainers 
		{ 
			get 
			{ 
				if (!_isInitialized)
					Initialize();

				return _containerCache.Values; 
			} 
		}

		/// <see cref="ITestContainerDiscoverer.TestContainersUpdated"/>
		public event EventHandler TestContainersUpdated;

		private void OnTestContainersUpdated()
		{
			var localEvent = TestContainersUpdated;
			if (localEvent != null)
				localEvent(this, EventArgs.Empty);
		}

		private IEnumerable<IVSTestContainer> DiscoverTestContainers(IVsProject project)
		{
			var files = project.Info().GetProjectItems().Where(IsScriptFile);
			return files.Select(file => _containerFactory(this, file, project));
		}

		private static bool IsScriptFile(string path)
		{
			// Comparison is reversed due to possible null reference (no extension).
			return TestScript.FileExtension.Equals(Path.GetExtension(path), StringComparison.OrdinalIgnoreCase);
		}

		private void AddContainerToCache(IVSTestContainer container, IVsProject project)
		{
			_containerCache[container.Source] = container;

			if (!_containersPerProject.ContainsKey(project))
				_containersPerProject[project] = new List<IVSTestContainer>();

			_containersPerProject[project].Add(container);
		}

		private void RemoveContainerFromCache(string source, IVsProject project)
		{
			IVSTestContainer throwaway;
			_containerCache.TryRemove(source, out throwaway);

			ICollection<IVSTestContainer> projectContainers;
			if (_containersPerProject.TryGetValue(project, out projectContainers))
				projectContainers.Remove(throwaway);
		}

		private void AddProjectToCache(IVsProject project)
		{
			var containers = DiscoverTestContainers(project);
			foreach (var container in containers)
				AddContainerToCache(container, project);
		}

		private void RemoveProjectFromCache(IVsProject project)
		{
			ICollection<IVSTestContainer> existingContainers;
			if (_containersPerProject.TryRemove(project, out existingContainers))
			{
				foreach (var container in existingContainers)
				{
					IVSTestContainer throwaway;
					_containerCache.TryRemove(container.Source, out throwaway);
				}
			}
		}

		#region IDisposable Implementation

		private void Dispose(bool disposing)
		{
			if (!_isDisposed)
			{
				if (disposing)
				{
					_solutionMonitor.SolutionChanged -= solutionMonitor_SolutionChanged;
					_solutionMonitor.ProjectChanged -= solutionMonitor_ProjectChanged;

					_projectFileMonitor.FileChanged -= projectFileMonitor_FileChanged;

					_fileWatcher.Changed -= fileWatcher_Changed;
					_fileWatcher.Error -= fileWatcher_Error;
					_fileWatcher.EnableRaisingEvents = false;
					_fileWatcher.Dispose();
				}

				_isDisposed = true;
			}
		}

		/// <see cref="IDisposable.Dispose"/>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~VSTestContainerDiscoverer()
		{
			Dispose(false);
		}

		private bool _isDisposed;

		#endregion IDisposable Implementation

		private DateTime _lastFileUpdate = DateTime.MinValue;

		private bool _isInitialized;

		private readonly ConcurrentDictionary<string, IVSTestContainer> _containerCache = 
			new ConcurrentDictionary<string, IVSTestContainer>(CaseInsensitiveEqualityComparer.Instance);
		private readonly ConcurrentDictionary<IVsProject, ICollection<IVSTestContainer>> _containersPerProject = 
			new ConcurrentDictionary<IVsProject, ICollection<IVSTestContainer>>();

		private readonly IServiceProvider _serviceProvider;
		private readonly ILogger _logger;
		private readonly ISolutionMonitor _solutionMonitor;
		private readonly IProjectFileMonitor _projectFileMonitor;
		private readonly Func<ITestContainerDiscoverer, string, IVsProject, IVSTestContainer> _containerFactory;
		private readonly IFileSystemWatcher _fileWatcher;
	}
}