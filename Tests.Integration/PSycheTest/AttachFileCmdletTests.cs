using System;
using System.IO;
using PSycheTest;
using PSycheTest.Core;
using PSycheTest.Runners.Framework.Utilities.InputOutput;
using Tests.Support;
using Xunit;

namespace Tests.Integration.PSycheTest
{
	public class AttachFileCmdletTests : CmdletTestBase<AttachFileCmdlet>, IDisposable
	{
		public AttachFileCmdletTests()
		{
			testContext.OutputDirectory = outputDir.Directory;
		}

		[Fact]
		public void Test_AttachFile()
		{
			// Arrange.
			Current.Shell.Runspace.SessionStateProxy.Variables().__testContext__ = testContext;

			AddCmdlet();
			AddParameter(p => p.Artifact, new Uri(artifactFile.File.FullName));

			// Act.
			Current.Shell.Invoke();

			// Assert.
			var file = Assert.Single(testContext.Artifacts);
			Assert.Equal(Path.Combine(outputDir.Directory.FullName, artifactFile.File.Name), file.OriginalString);
		}

		[Fact]
		public void Test_AttachFile_With_Relative_Path()
		{
			// Arrange.
			Current.Shell.Runspace.SessionStateProxy.Variables().__testContext__ = testContext;
			Current.Shell.Runspace.SessionStateProxy.Path.SetLocation(artifactFile.File.Directory.FullName);

			AddCmdlet();
			AddParameter(p => p.Artifact, new Uri(Path.Combine(@".\", artifactFile.File.Name), UriKind.Relative));

			// Act.
			Current.Shell.Invoke();

			// Assert.
			var file = Assert.Single(testContext.Artifacts);
			Assert.Equal(Path.Combine(outputDir.Directory.FullName, artifactFile.File.Name), file.LocalPath);
		}

		public void Dispose()
		{
			artifactFile.Dispose();
			outputDir.Dispose();
		}

		private readonly TemporaryFile artifactFile = new TemporaryFile().Touch(); 
		private readonly TemporaryDirectory outputDir = new TemporaryDirectory();

		private readonly TestExecutionContext testContext = new TestExecutionContext();
	}
}