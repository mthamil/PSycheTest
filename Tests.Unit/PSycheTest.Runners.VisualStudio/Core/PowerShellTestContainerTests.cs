using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestWindow.Extensibility;
using Moq;
using PSycheTest.Runners.Framework.Utilities.InputOutput;
using PSycheTest.Runners.VisualStudio.Core;
using Tests.Support;
using Xunit;

namespace Tests.Unit.PSycheTest.Runners.VisualStudio.Core
{
	public class PowerShellTestContainerTests : IDisposable
	{
		public PowerShellTestContainerTests()
		{
			originalProjectInfoFactory = VisualStudioHierarchyExtensions.ProjectInfoFactory;
			VisualStudioHierarchyExtensions.ProjectInfoFactory = p => projects[p];
		}

		[Fact]
		public void Test_Initialization()
		{
			// Arrange.
			var discoverer = Mock.Of<ITestContainerDiscoverer>();
			string scriptPath = @".\test.ps1";
			var project = Mock.Of<IVsProject>();
			projects[project] = Mock.Of<IProjectInfo>(p => p.File == new FileInfo("Project"));

			// Act.
			var container = new PowerShellTestContainer(
				discoverer, scriptPath, project);

			// Assert.
			Assert.Equal(discoverer, container.Discoverer);
			Assert.Equal(@".\test.ps1", container.Source);
			Assert.Equal(project, container.Project);
			Assert.Empty(container.DebugEngines);
			Assert.Equal(FrameworkVersion.None, container.TargetFramework);
			Assert.Equal(Architecture.AnyCPU, container.TargetPlatform);
		}

		[Fact]
		public void Test_CompareTo()
		{
			// Arrange.
			var discoverer = Mock.Of<ITestContainerDiscoverer>();

			var project = Mock.Of<IVsProject>();
			projects[project] = Mock.Of<IProjectInfo>(p => p.File == new FileInfo("Project"));

			using (var script = new TemporaryFile().Touch())
			{
				var container = new PowerShellTestContainer(
					discoverer, script.File.FullName, project);

				var otherContainer = new PowerShellTestContainer(
					discoverer, script.File.FullName, project);

				// Act.
				int result = container.CompareTo(otherContainer);

				// Assert.
				Assert.Equal(0, result);
			}
		}

		[Fact]
		public void Test_CompareTo_Null()
		{
			// Arrange.
			var discoverer = Mock.Of<ITestContainerDiscoverer>();
			var project = Mock.Of<IVsProject>();
			projects[project] = Mock.Of<IProjectInfo>();

			using (var script = new TemporaryFile().Touch())
			{
				var container = new PowerShellTestContainer(
					discoverer, script.File.FullName, project);

				// Act.
				int result = container.CompareTo(null);

				// Assert.
				Assert.Equal(-1, result);
			}
		}

		[Fact]
		public void Test_CompareTo_Different_Sources()
		{
			// Arrange.
			var discoverer = Mock.Of<ITestContainerDiscoverer>();
			var project = Mock.Of<IVsProject>();
			projects[project] = Mock.Of<IProjectInfo>();

			using (var script = new TemporaryFile().Touch())
			using (var otherScript = new TemporaryFile().Touch())
			{
				var container = new PowerShellTestContainer(
					discoverer, script.File.FullName, project);

				var otherContainer = new PowerShellTestContainer(
					discoverer, otherScript.File.FullName, project);

				// Act.
				int result = container.CompareTo(otherContainer);

				// Assert.
				Assert.NotEqual(0, result);
			}
		}

		[Fact]
		public void Test_CompareTo_Different_Projects()
		{
			// Arrange.
			var discoverer = Mock.Of<ITestContainerDiscoverer>();

			var project = Mock.Of<IVsProject>();
			projects[project] = Mock.Of<IProjectInfo>(p => p.File == new FileInfo("Project1"));

			var otherProject = Mock.Of<IVsProject>();
			projects[otherProject] = Mock.Of<IProjectInfo>(p => p.File == new FileInfo("Project2"));

			using (var script = new TemporaryFile().Touch())
			{
				var container = new PowerShellTestContainer(
					discoverer, script.File.FullName, project);

				var otherContainer = new PowerShellTestContainer(
					discoverer, script.File.FullName, otherProject);

				// Act.
				int result = container.CompareTo(otherContainer);

				// Assert.
				Assert.NotEqual(0, result);
			}
		}

		public void Dispose()
		{
			VisualStudioHierarchyExtensions.ProjectInfoFactory = originalProjectInfoFactory;
		}

		private readonly IDictionary<IVsProject, IProjectInfo> projects = new Dictionary<IVsProject, IProjectInfo>();

		private readonly Func<IVsProject, IProjectInfo> originalProjectInfoFactory;
	}
}