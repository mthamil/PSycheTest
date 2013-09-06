using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Moq;
using PSycheTest.Runners.Framework;
using PSycheTest.Runners.Framework.Utilities.InputOutput;
using PSycheTest.Runners.VisualStudio;
using PSycheTest.Runners.VisualStudio.Core;
using Tests.Support;
using Xunit;

namespace Tests.Unit.PSycheTest.Runners.VisualStudio
{
	public class VSTestExecutorTests : IDisposable
	{
		public VSTestExecutorTests()
		{
			modulePath = new TemporaryDirectory();

			testSettings.SetupGet(ts => ts.Modules).Returns(new string[0]);

			var settingsService = new Mock<IPSycheTestSettingsService>();
			settingsService.SetupGet(ss => ss.Settings).Returns(testSettings.Object);
			
			runContext = Mock.Of<IRunContext>(rc =>
				rc.SolutionDirectory == modulePath.Directory.FullName &&
				rc.TestRunDirectory == Directory.GetCurrentDirectory() &&
				rc.RunSettings == Mock.Of<IRunSettings>(rs =>
				rs.GetSettings(PSycheTestRunSettings.SettingsProviderName) ==
					settingsService.As<ISettingsProvider>().Object));

			vsExecutor = new VSTestExecutor(_ => executor.Object, _ => discoverer.Object);

			executor.SetupProperty(e => e.OutputDirectory);

			executor.Setup(e => e.ExecuteAsync(
				It.IsAny<IEnumerable<ITestScript>>(),
				It.IsAny<Predicate<ITestFunction>>(),
				It.IsAny<CancellationToken>()))
			        .Returns(Task.FromResult<object>(null));

			discoverer.Setup(d => d.Discover(It.IsAny<IEnumerable<FileInfo>>()))
					  .Returns((IEnumerable<FileInfo> files) =>
						  files.Select(f => Mock.Of<ITestScript>(ts => ts.Source == f)).ToList());
		}

		[Fact]
		public void Test_RunTests_From_Sources()
		{
			// Arrange.
			var sources = new[] { @"C:\test1.ps1", @"C:\test2.ps1" };

			// Act.
			vsExecutor.RunTests(sources, runContext, frameworkHandle.Object);

			// Assert.
			discoverer.Verify(d => d.Discover(It.Is<IEnumerable<FileInfo>>(fs => 
				fs.Select(f => f.FullName).SequenceEqual(sources))));

			executor.Verify(e => e.ExecuteAsync(It.Is<IEnumerable<ITestScript>>(ts => 
					ts.Select(t => t.Source.FullName).SequenceEqual(new[] { @"C:\test1.ps1", @"C:\test2.ps1" })), 
				It.IsAny<Predicate<ITestFunction>>(),
				It.IsAny<CancellationToken>()), Times.Once());
		}

		[Fact]
		public void Test_RunTests_From_TestCases()
		{
			// Arrange.
			var sources = new[] { @"C:\test1.ps1", @"C:\test2.ps1" };

			var testCases = sources.Select(source => new TestCase(source, executorUri, source)).ToList();

			// Act.
			vsExecutor.RunTests(testCases, runContext, frameworkHandle.Object);

			// Assert.
			executor.Verify(e => e.ExecuteAsync(It.Is<IEnumerable<ITestScript>>(ts =>
					ts.Select(t => t.Source.FullName).SequenceEqual(sources)),
				It.IsAny<Predicate<ITestFunction>>(),
				It.IsAny<CancellationToken>()), Times.Once());
		}

		[Fact]
		public void Test_RunTests_From_TestCases_Multiple_Tests_Per_Script()
		{
			// Arrange.
			var sources = new[] { @"C:\test1.ps1", @"C:\test2.ps1" };

			var testCases = sources
				.SelectMany(
					source => Enumerable.Range(1, 2), 
					(source, index) => new TestCase(source + ":" + index, executorUri, source)).ToList();

			// Act.
			vsExecutor.RunTests(testCases, runContext, frameworkHandle.Object);

			// Assert.
			executor.Verify(e => e.ExecuteAsync(It.Is<IEnumerable<ITestScript>>(ts =>
					ts.Select(t => t.Source.FullName).SequenceEqual(sources)),
				It.IsAny<Predicate<ITestFunction>>(),
				It.IsAny<CancellationToken>()), Times.Once());
		}

		[Fact]
		public void Test_Modules()
		{
			// Arrange.
			testSettings.SetupGet(ts => ts.Modules).Returns(new[] { "Module1.psd1", "Module2.psd2" });
			executor.SetupGet(e => e.InitialModules).Returns(new List<string>());

			var sources = new[] { @"C:\test1.ps1" };

			// Act.
			vsExecutor.RunTests(sources, runContext, frameworkHandle.Object);

			// Assert.
			Assert.Equal(new[] { "Module1.psd1", "Module2.psd2" }, executor.Object.InitialModules.ToArray());
		}

		public void Dispose()
		{
			modulePath.Dispose();
		}

		private readonly VSTestExecutor vsExecutor;

		private readonly Mock<IFrameworkHandle> frameworkHandle = new Mock<IFrameworkHandle>();
		private readonly IRunContext runContext;
		private readonly Mock<IPSycheTestRunSettings> testSettings = new Mock<IPSycheTestRunSettings> { DefaultValue = DefaultValue.Empty };
 		private readonly Mock<IPowerShellTestExecutor> executor = new Mock<IPowerShellTestExecutor>();
		private readonly Mock<IPowerShellTestDiscoverer> discoverer = new Mock<IPowerShellTestDiscoverer>();

		private readonly TemporaryDirectory modulePath;

		private static readonly Uri executorUri = new Uri("executor://test");
	}
}