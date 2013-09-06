using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestWindow.Extensibility;
using Microsoft.VisualStudio.TestWindow.Extensibility.Model;

namespace PSycheTest.Runners.VisualStudio.Core
{
	/// <summary>
	/// Represents a PowerShell script containing tests.
	/// </summary>
	public class PowerShellTestContainer : IVSTestContainer
	{
		/// <summary>
		/// Initializes a new <see cref="PowerShellTestContainer"/>.
		/// </summary>
		/// <param name="containerDiscoverer">The associated discoverer</param>
		/// <param name="scriptPath">A string representing the container source, ie. a file path</param>
		/// <param name="project">A source's parent project</param>
		public PowerShellTestContainer(ITestContainerDiscoverer containerDiscoverer, string scriptPath, IVsProject project)
			: this(containerDiscoverer, scriptPath, project, Enumerable.Empty<Guid>()) { }

		/// <summary>
		/// Initializes a new <see cref="PowerShellTestContainer"/>.
		/// </summary>
		/// <param name="containerDiscoverer">The associated discoverer</param>
		/// <param name="scriptPath">A string representing the container source, ie. a file path</param>
		/// <param name="project">A source's parent project</param>
		/// <param name="debugEngines">Any debugging engines</param>
		public PowerShellTestContainer(ITestContainerDiscoverer containerDiscoverer, string scriptPath, IVsProject project, IEnumerable<Guid> debugEngines)
		{
			_containerDiscoverer = ValidateArg.NotNull(containerDiscoverer, "containerDiscoverer"); ;
			_scriptPath = ValidateArg.NotNull(scriptPath, "scriptPath");
			Project = ValidateArg.NotNull(project, "project");

			_projectFile = project.Info().File;
			TargetFramework = FrameworkVersion.None;
			TargetPlatform = Architecture.AnyCPU;
			DebugEngines = debugEngines;
			
			_timestamp = GetTimestamp();
		}

		/// <summary>
        /// Initializes a new <see cref="PowerShellTestContainer"/> with an existing instance.
        /// </summary>
		/// <param name="source">The <see cref="PowerShellTestContainer"/> to copy</param>
		private PowerShellTestContainer(PowerShellTestContainer source)
            : this(source.Discoverer, source.Source, source.Project)
		{
			_timestamp = source._timestamp;
		}

		/// <see cref="IVSTestContainer.Project"/>
		public IVsProject Project { get; private set; }

		/// <see cref="ITestContainer.Discoverer"/>
		public ITestContainerDiscoverer Discoverer 
		{
			get { return _containerDiscoverer; }
		}

		/// <see cref="ITestContainer.Source"/>
		public string Source 
		{
			get { return _scriptPath; }
		}

		/// <see cref="ITestContainer.DebugEngines"/>
		public IEnumerable<Guid> DebugEngines { get; private set; }

		/// <see cref="ITestContainer.TargetFramework"/>
		public FrameworkVersion TargetFramework { get; private set; }

		/// <see cref="ITestContainer.TargetPlatform"/>
		public Architecture TargetPlatform { get; private set; }

		/// <see cref="ITestContainer.IsAppContainerTestContainer"/>
		public bool IsAppContainerTestContainer { get { return false; } }

		/// <see cref="ITestContainer.DeployAppContainer"/>
		public IDeploymentData DeployAppContainer()
		{
			return null;
		}

		/// <see cref="ITestContainer.CompareTo"/>
		public int CompareTo(ITestContainer other)
		{
			var testContainer = other as PowerShellTestContainer;
			if (testContainer == null)
				return -1;

			var sourcesEqual = String.Compare(Source, testContainer.Source, StringComparison.OrdinalIgnoreCase);
			if (sourcesEqual != 0)
				return sourcesEqual;

			var projectsEqual = String.Compare(_projectFile.FullName, testContainer._projectFile.FullName, StringComparison.Ordinal);
			if (projectsEqual != 0)
				return projectsEqual;

			return _timestamp.CompareTo(testContainer._timestamp);
		}

		/// <see cref="ITestContainer.Snapshot"/>
		public ITestContainer Snapshot()
		{
			return new PowerShellTestContainer(this);
		}

		/// <summary>
		/// Determines a timestamp for the container.
		/// </summary>
		private DateTime GetTimestamp()
		{
			if (!String.IsNullOrEmpty(Source) && File.Exists(Source))
				return File.GetLastWriteTime(Source);

			return DateTime.MinValue;
		}

		private DateTime _timestamp;
		private readonly string _scriptPath;
		private readonly FileInfo _projectFile;
		private readonly ITestContainerDiscoverer _containerDiscoverer;
	}
}