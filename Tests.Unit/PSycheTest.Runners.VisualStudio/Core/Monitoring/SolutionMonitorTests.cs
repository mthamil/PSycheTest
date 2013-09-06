using System;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Moq;
using PSycheTest.Runners.VisualStudio.Core.Monitoring;
using Tests.Support;
using Xunit;

namespace Tests.Unit.PSycheTest.Runners.VisualStudio.Core.Monitoring
{
	public class SolutionMonitorTests
	{
		public SolutionMonitorTests()
		{
			serviceProvider.Setup(sp => sp.GetService(typeof(SVsSolution)))
			               .Returns(solution.Object);

			monitor = new SolutionMonitor(serviceProvider.Object);
		}

		[Fact]
		public void Test_StartMonitoring()
		{
			// Act.
			monitor.StartMonitoring();

			// Assert.
			uint cookie;
			solution.Verify(s => s.AdviseSolutionEvents(monitor, out cookie));
		}

		[Fact]
		public void Test_StopMonitoring()
		{
			// Arrange.
			uint cookie = 123;
			solution.Setup(s => s.AdviseSolutionEvents(monitor, out cookie));
			monitor.StartMonitoring();

			// Act.
			monitor.StopMonitoring();

			// Assert.
			solution.Verify(s => s.UnadviseSolutionEvents(123));
		}

		[Fact]
		public void Test_OnAfterOpenSolution()
		{
			// Act/Assert.
			int result = Int32.MinValue;
			AssertThat.DoesNotRaise<ISolutionMonitor>(
				monitor, sm => sm.SolutionChanged += null,
				() => result = monitor.OnAfterOpenSolution(null, 0));

			Assert.Equal(VSConstants.S_OK, result);
		}

		[Fact]
		public void Test_OnAfterCloseSolution()
		{
			// Act/Assert.
			int result = Int32.MinValue;
			var args = AssertThat.RaisesWithEventArgs<ISolutionMonitor, SolutionChangedEventArgs>(
				monitor, sm => sm.SolutionChanged += null,
				() => result = monitor.OnAfterCloseSolution(null));

			Assert.Equal(VSConstants.S_OK, result);
			Assert.Equal(SolutionChangeType.Unloaded, args.ChangeType);
			Assert.Equal(solution.Object, args.Solution);
		}

		[Fact]
		public void Test_OnAfterBackgroundSolutionLoadComplete()
		{
			// Act/Assert.
			int result = Int32.MinValue;
			var args = AssertThat.RaisesWithEventArgs<ISolutionMonitor, SolutionChangedEventArgs>(
				monitor, sm => sm.SolutionChanged += null,
				() => result = monitor.OnAfterBackgroundSolutionLoadComplete());

			Assert.Equal(VSConstants.S_OK, result);
			Assert.Equal(SolutionChangeType.Loaded, args.ChangeType);
			Assert.Equal(solution.Object, args.Solution);
		}

		[Fact]
		public void Test_OnAfterLoadProject()
		{
			// Act/Assert.
			int result = Int32.MinValue;
			var args = AssertThat.RaisesWithEventArgs<ISolutionMonitor, ProjectChangedEventArgs>(
				monitor, sm => sm.ProjectChanged += null,
				() => result = monitor.OnAfterLoadProject(null, project.As<IVsHierarchy>().Object));

			Assert.Equal(VSConstants.S_OK, result);
			Assert.Equal(ProjectChangeType.Loaded, args.ChangeType);
			Assert.Equal(solution.Object, args.Solution);
			Assert.Equal(project.Object, args.Project);
		}

		[Fact]
		public void Test_OnBeforeUnloadProject()
		{
			// Act/Assert.
			int result = Int32.MinValue;
			var args = AssertThat.RaisesWithEventArgs<ISolutionMonitor, ProjectChangedEventArgs>(
				monitor, sm => sm.ProjectChanged += null,
				() => result = monitor.OnBeforeUnloadProject(project.As<IVsHierarchy>().Object, null));

			Assert.Equal(VSConstants.S_OK, result);
			Assert.Equal(ProjectChangeType.Unloaded, args.ChangeType);
			Assert.Equal(solution.Object, args.Solution);
			Assert.Equal(project.Object, args.Project);
		}

		private readonly SolutionMonitor monitor;

		private readonly Mock<IVsProject> project = new Mock<IVsProject>();
		private readonly Mock<IVsSolution> solution = new Mock<IVsSolution>();
		private readonly Mock<IServiceProvider> serviceProvider = new Mock<IServiceProvider>();
	}
}