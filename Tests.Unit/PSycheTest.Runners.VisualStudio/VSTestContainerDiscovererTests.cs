using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestWindow.Extensibility;
using Moq;
using PSycheTest.Runners.VisualStudio;
using PSycheTest.Runners.VisualStudio.Core;
using PSycheTest.Runners.VisualStudio.Core.Monitoring;
using PSycheTest.Runners.VisualStudio.Core.Utilities.InputOutput;
using Tests.Support;
using Xunit;
using ILogger = PSycheTest.Runners.Framework.ILogger;

namespace Tests.Unit.PSycheTest.Runners.VisualStudio
{
	public class VSTestContainerDiscovererTests : IDisposable
	{
		public VSTestContainerDiscovererTests()
		{
			originalSolutionInfoFactory = VisualStudioHierarchyExtensions.SolutionInfoFactory;
			VisualStudioHierarchyExtensions.SolutionInfoFactory = p => solutionInfo;

			originalProjectInfoFactory = VisualStudioHierarchyExtensions.ProjectInfoFactory;
			VisualStudioHierarchyExtensions.ProjectInfoFactory = p => projects[p];

			solutionInfo = Mock.Of<ISolutionInfo>(si =>
				si.GetProjects() == projects.Keys &&
				si.GetDirectory() == new DirectoryInfo(@"C:\Solution"));

			serviceProvider.Setup(s => s.GetService(typeof(SVsSolution)))
			               .Returns(solution.Object);

			discoverer = new VSTestContainerDiscoverer(serviceProvider.Object, logger.Object, 
				solutionMonitor.Object, projectMonitor.Object, fileWatcher.Object, CreateContainer);
		}

		[Fact]
		public void Test_SolutionLoaded()
		{
			// Arrange.
			var projectInfo = Mock.Of<IProjectInfo>(p => 
				p.GetProjectItems() == new[] { @"C:\test.ps2", @"file.txt", @"C:\test.ps1" } && p.IsPhysicalProject == true);
			projects[Mock.Of<IVsProject>()] = projectInfo;

			// Act.
			AssertThat.Raises<ITestContainerDiscoverer>(discoverer, d => d.TestContainersUpdated += null,
				() => solutionMonitor.Raise(sm => sm.SolutionChanged += null,
					new SolutionChangedEventArgs(solution.Object, SolutionChangeType.Loaded)));

			// Assert.
			var container = Assert.Single(discoverer.TestContainers);
			Assert.Equal(@"C:\test.ps1", container.Source);
			Assert.Equal(discoverer, container.Discoverer);
		}

		[Fact]
		public void Test_SolutionLoaded_NonPhysical_Projects_Filtered_Out()
		{
			// Arrange.
			var projectInfo = Mock.Of<IProjectInfo>(p =>
				p.GetProjectItems() == new[] { @"C:\test.ps2", @"file.txt", @"C:\test.ps1" } && p.IsPhysicalProject == false);
			projects[Mock.Of<IVsProject>()] = projectInfo;

			// Act.
			AssertThat.Raises<ITestContainerDiscoverer>(discoverer, d => d.TestContainersUpdated += null,
				() => solutionMonitor.Raise(sm => sm.SolutionChanged += null,
					new SolutionChangedEventArgs(solution.Object, SolutionChangeType.Loaded)));

			// Assert.
			Assert.Empty(discoverer.TestContainers);
		}

		[Fact]
		public void Test_SolutionUnloaded()
		{
			// Arrange.
			InitializeWith(Proj(Mock.Of<IVsProject>(), Mock.Of<IProjectInfo>(p => 
				p.GetProjectItems() == new[] { @"C:\test.ps1" } && p.IsPhysicalProject == true)));

			// Act.
			solutionMonitor.Raise(sm => sm.SolutionChanged += null,
				new SolutionChangedEventArgs(solution.Object, SolutionChangeType.Unloaded));

			projects.Clear();

			// Assert.
			Assert.Empty(discoverer.TestContainers);
		}

		[Fact]
		public void Test_SolutionLoaded_Starts_Monitoring_File_Changes()
		{
			// Act.
			solutionMonitor.Raise(sm => sm.SolutionChanged += null,
				new SolutionChangedEventArgs(solution.Object, SolutionChangeType.Loaded));

			// Assert.
			fileWatcher.VerifySet(fw => fw.EnableRaisingEvents = true);
			fileWatcher.VerifySet(fw => fw.IncludeSubdirectories = true);
			fileWatcher.VerifySet(fw => fw.Path = @"C:\Solution");
			fileWatcher.VerifySet(fw => fw.Filter = "*.ps1");
			fileWatcher.VerifySet(fw => fw.NotifyFilter = NotifyFilters.LastWrite);
		}

		[Fact]
		public void Test_SolutionUnloaded_Stops_Monitoring_File_Changes()
		{
			// Act.
			solutionMonitor.Raise(sm => sm.SolutionChanged += null,
				new SolutionChangedEventArgs(solution.Object, SolutionChangeType.Unloaded));

			// Assert.
			fileWatcher.VerifySet(fw => fw.EnableRaisingEvents = false);
		}

		[Fact]
		public void Test_ProjectLoaded()
		{
			// Arrange.
			var project = Mock.Of<IVsProject>();
			var projectInfo = Mock.Of<IProjectInfo>(p => 
				p.GetProjectItems() == new[] { @"C:\test1.ps1" } && p.IsPhysicalProject == true);
			projects[project] = projectInfo;

			// Act.
			AssertThat.Raises<ITestContainerDiscoverer>(discoverer, d => d.TestContainersUpdated += null,
				() => solutionMonitor.Raise(sm => sm.ProjectChanged += null,
					new ProjectChangedEventArgs(solution.Object, project, ProjectChangeType.Loaded)));

			var containers = discoverer.TestContainers.OrderBy(t => t.Source).ToList();

			// Assert.
			var container = Assert.Single(containers);
			Assert.Equal(@"C:\test1.ps1", container.Source);
		}

		[Fact]
		public void Test_ProjectReloaded()
		{
			// Arrange.
			var projectInfo1 = Mock.Of<IProjectInfo>(p => 
				p.GetProjectItems() == new[] { @"C:\test1.ps1" } && p.IsPhysicalProject == true);

			var project2 = Mock.Of<IVsProject>();
			var project2Files = new List<string> { @"C:\test2.ps1", @"C:\test3.ps1" };
			var projectInfo2 = Mock.Of<IProjectInfo>(p => p.GetProjectItems() == project2Files);

			InitializeWith(Proj(Mock.Of<IVsProject>(), projectInfo1), Proj(project2, projectInfo2));

			project2Files.Remove(@"C:\test2.ps1");	// Remove a file from project 2.

			// Act.
			solutionMonitor.Raise(sm => sm.ProjectChanged += null,
				new ProjectChangedEventArgs(solution.Object, project2, ProjectChangeType.Loaded));

			var containers = discoverer.TestContainers.OrderBy(t => t.Source).ToList();

			// Assert.
			Assert.Equal(2, containers.Count);
			Assert.Equal(@"C:\test1.ps1", containers.First().Source);
			Assert.Equal(@"C:\test3.ps1", containers.Last().Source);
		}

		[Fact]
		public void Test_ProjectUnloaded()
		{
			// Arrange.
			var project1 = Mock.Of<IVsProject>();
			var projectInfo1 = Mock.Of<IProjectInfo>(p => 
				p.GetProjectItems() == new[] { @"C:\test1.ps1", @"C:\test2.ps1" } && p.IsPhysicalProject == true);

			var project2Files = new List<string> { @"C:\test3.ps1" };
			var projectInfo2 = Mock.Of<IProjectInfo>(p => p.GetProjectItems() == project2Files && p.IsPhysicalProject == true);

			InitializeWith(Proj(project1, projectInfo1), Proj(Mock.Of<IVsProject>(), projectInfo2));

			projects.Remove(project1);

			// Act.
			AssertThat.Raises<ITestContainerDiscoverer>(discoverer, d => d.TestContainersUpdated += null,
				() => solutionMonitor.Raise(sm => sm.ProjectChanged += null,
					new ProjectChangedEventArgs(solution.Object, project1, ProjectChangeType.Unloaded)));

			// Assert.
			var container = Assert.Single(discoverer.TestContainers);
			Assert.Equal(@"C:\test3.ps1", container.Source);
		}

		[Fact]
		public void Test_File_Removed()
		{
			// Arrange.
			var project = Mock.Of<IVsProject>();
			var projectInfo = Mock.Of<IProjectInfo>(p => 
				p.GetProjectItems() == new[] { @"C:\test1.ps1", @"C:\test2.ps1", @"C:\test3.ps1" } && p.IsPhysicalProject == true);

			InitializeWith(Proj(project, projectInfo));

			// Act.
			AssertThat.Raises<ITestContainerDiscoverer>(discoverer, d => d.TestContainersUpdated += null,
				() => projectMonitor.Raise(pm => pm.FileChanged += null, 
					new ProjectFileChangedEventArgs(project, @"C:\test2.ps1", FileChangeType.Removed)));

			var containers = discoverer.TestContainers.OrderBy(t => t.Source).ToList();

			// Assert.
			Assert.Equal(2, containers.Count);
			Assert.Equal(@"C:\test1.ps1", containers.First().Source);
			Assert.Equal(@"C:\test3.ps1", containers.Last().Source);
		}

		[Fact]
		public void Test_File_Added()
		{
			// Arrange.
			var project = Mock.Of<IVsProject>();
			var projectInfo = Mock.Of<IProjectInfo>(p => 
				p.GetProjectItems() == new[] { @"C:\test1.ps1", @"C:\test2.ps1" } && p.IsPhysicalProject == true);

			InitializeWith(Proj(project, projectInfo));

			// Act.
			AssertThat.Raises<ITestContainerDiscoverer>(discoverer, d => d.TestContainersUpdated += null,
				() => projectMonitor.Raise(pm => pm.FileChanged += null, 
					new ProjectFileChangedEventArgs(project, @"C:\test3.ps1", FileChangeType.Added)));

			var containers = discoverer.TestContainers.OrderBy(t => t.Source).ToList();

			// Assert.
			Assert.Equal(3, containers.Count);
			Assert.Equal(@"C:\test1.ps1", containers[0].Source);
			Assert.Equal(@"C:\test2.ps1", containers[1].Source);
			Assert.Equal(@"C:\test3.ps1", containers[2].Source);
		}

		[Fact]
		public void Test_File_Changed()
		{
			// Arrange.
			var project = Mock.Of<IVsProject>();
			var projectInfo = Mock.Of<IProjectInfo>(p =>
				p.GetProjectItems() == new[] { @"C:\test1.ps1", @"C:\test2.ps1" } && p.IsPhysicalProject == true);

			InitializeWith(Proj(project, projectInfo));

			// Act.
			AssertThat.Raises<ITestContainerDiscoverer>(discoverer, d => d.TestContainersUpdated += null,
				() => fileWatcher.Raise(fw => fw.Changed += null, 
					new FileSystemEventArgs(WatcherChangeTypes.Changed, @"C:\", "test1.ps1")));

			var containers = discoverer.TestContainers.OrderBy(t => t.Source).ToList();

			// Assert.
			Assert.Equal(2, containers.Count);
			Assert.Equal(@"C:\test1.ps1", containers[0].Source);
			Assert.Equal(@"C:\test2.ps1", containers[1].Source);
		}

		[Fact]
		public void Test_File_Changed_When_File_Not_In_Solution()
		{
			// Arrange.
			var project = Mock.Of<IVsProject>();
			var projectInfo = Mock.Of<IProjectInfo>(p => 
				p.GetProjectItems() == new[] { @"C:\test1.ps1", @"C:\test2.ps1" } && p.IsPhysicalProject == true);

			InitializeWith(Proj(project, projectInfo));

			// Act.
			AssertThat.DoesNotRaise<ITestContainerDiscoverer>(discoverer, d => d.TestContainersUpdated += null,
				() => fileWatcher.Raise(fw => fw.Changed += null,
					new FileSystemEventArgs(WatcherChangeTypes.Changed, @"C:\", "test3.ps1")));

			var containers = discoverer.TestContainers.OrderBy(t => t.Source).ToList();

			// Assert.
			Assert.Equal(2, containers.Count);
			Assert.Equal(@"C:\test1.ps1", containers[0].Source);
			Assert.Equal(@"C:\test2.ps1", containers[1].Source);
		}

		[Fact]
		public void Test_FileSystemWatcher_Errors_Logged()
		{
			// Arrange.
			var exception = new InvalidOperationException();

			// Act.
			fileWatcher.Raise(fsw => fsw.Error += null, new ErrorEventArgs(exception));

			// Assert.
			logger.Verify(l => l.Error(It.IsAny<string>(), It.IsAny<object[]>()), Times.Once());
		}

		[Fact]
		public void Test_Dispose()
		{
			// Act.
			discoverer.Dispose();

			// Assert.
			fileWatcher.VerifySet(fw => fw.EnableRaisingEvents = false);
			fileWatcher.Verify(fw => fw.Dispose());
		}

		private void InitializeWith(params Tuple<IVsProject, IProjectInfo>[] initialProjects)
		{
			foreach (var initialProject in initialProjects)
				projects[initialProject.Item1] = initialProject.Item2;

			solutionMonitor.Raise(sm => sm.SolutionChanged += null,
				new SolutionChangedEventArgs(solution.Object, SolutionChangeType.Loaded));
		}

		private static Tuple<IVsProject, IProjectInfo> Proj(IVsProject project, IProjectInfo info)
		{
			return Tuple.Create(project, info);
		}

		private static IVSTestContainer CreateContainer(ITestContainerDiscoverer discoverer, string source, IVsProject project)
		{
			return Mock.Of<IVSTestContainer>(c => c.Discoverer == discoverer && c.Source == source && c.Project == project);
		}

		public void Dispose()
		{
			VisualStudioHierarchyExtensions.SolutionInfoFactory = originalSolutionInfoFactory;
			VisualStudioHierarchyExtensions.ProjectInfoFactory = originalProjectInfoFactory;
		}

		private readonly VSTestContainerDiscoverer discoverer;

		private readonly Mock<IVsSolution> solution = new Mock<IVsSolution>();
		private readonly ISolutionInfo solutionInfo;
		private readonly IDictionary<IVsProject, IProjectInfo> projects = new Dictionary<IVsProject, IProjectInfo>();
		
		private readonly Mock<IServiceProvider> serviceProvider = new Mock<IServiceProvider>();
		private readonly Mock<ILogger> logger = new Mock<ILogger>();
		private readonly Mock<ISolutionMonitor> solutionMonitor = new Mock<ISolutionMonitor>();
		private readonly Mock<IProjectFileMonitor> projectMonitor = new Mock<IProjectFileMonitor>();
		private readonly Mock<IFileSystemWatcher> fileWatcher = new Mock<IFileSystemWatcher>();

		private readonly Func<IVsSolution, ISolutionInfo> originalSolutionInfoFactory;
		private readonly Func<IVsProject, IProjectInfo> originalProjectInfoFactory;
	}
}