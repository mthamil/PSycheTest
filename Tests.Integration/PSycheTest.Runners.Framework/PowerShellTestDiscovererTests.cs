using System.Linq;
using Moq;
using PSycheTest.Runners.Framework;
using Tests.Integration.TestScripts;
using Xunit;

namespace Tests.Integration.PSycheTest.Runners.Framework
{
	public class PowerShellTestDiscovererTests
	{
		public PowerShellTestDiscovererTests()
		{
			discoverer  = new PowerShellTestDiscoverer(logger.Object);
		}

		[Fact]
		public void Test_Discover_With_Single_File_Single_Test()
		{
			// Arrange.
			var file = ScriptFiles.HasOneTest;

			// Act.
			var testScript = discoverer.Discover(file);

			// Assert.
			Assert.True(testScript.HasValue);
			var test = Assert.Single(testScript.Value.Tests);

			Assert.Equal(file.FullName + ":Test-AssertTrue-Failure", test.UniqueName);
			Assert.Equal("Test-AssertTrue-Failure", test.DisplayName);
			Assert.Equal("Test!", test.CustomTitle);
			Assert.Equal(1, test.Source.LineNumber);
			Assert.Equal(1, test.Source.ColumnNumber);
			Assert.Equal(file.FullName, test.Source.File.FullName);

			Assert.False(testScript.Value.TestSetup.HasValue);
			Assert.False(testScript.Value.TestCleanup.HasValue);
		}

		[Fact]
		public void Test_Discover_With_Single_File_Multiple_Tests()
		{
			// Arrange.
			var file = ScriptFiles.HasMultipleTests;

			// Act.
			var testScript = discoverer.Discover(file);

			// Assert.
			Assert.True(testScript.HasValue);
			Assert.Equal(4, testScript.Value.Tests.Count());
		}

		[Fact]
		public void Test_Discover_With_Single_File_With_Skipped_Test()
		{
			// Arrange.
			var file = ScriptFiles.HasSkippedTest;

			// Act.
			var testScript = discoverer.Discover(file);

			// Assert.
			Assert.True(testScript.HasValue);
			Assert.Equal(2, testScript.Value.Tests.Count());

			var skippedTest = testScript.Value.Tests.First();
			Assert.Equal(file.FullName + ":Test-ShouldBeSkipped", skippedTest.UniqueName);
			Assert.Equal("Test-ShouldBeSkipped", skippedTest.DisplayName);
			Assert.Equal("Should be skipped", skippedTest.CustomTitle);
			Assert.True(skippedTest.ShouldSkip);
			Assert.Equal("I said so.", skippedTest.SkipReason);
			Assert.Equal(1, skippedTest.Source.LineNumber);
			Assert.Equal(1, skippedTest.Source.ColumnNumber);
			Assert.Equal(file.FullName, skippedTest.Source.File.FullName);

			var normalTest = testScript.Value.Tests.Last();
			Assert.Equal(file.FullName + ":Test-OneEqualsOne", normalTest.UniqueName);
			Assert.Equal("Test-OneEqualsOne", normalTest.DisplayName);
			Assert.Equal("Test 1 = 1", normalTest.CustomTitle);
			Assert.False(normalTest.ShouldSkip);
			Assert.Equal(string.Empty, normalTest.SkipReason);
			Assert.Equal(9, normalTest.Source.LineNumber);
			Assert.Equal(1, normalTest.Source.ColumnNumber);
			Assert.Equal(file.FullName, normalTest.Source.File.FullName);
		}

		[Fact]
		public void Test_Discover_With_Setup_And_Cleanup()
		{
			// Arrange.
			var file = ScriptFiles.HasSetupAndCleanup;

			// Act.
			var testScript = discoverer.Discover(file);

			// Assert.
			Assert.True(testScript.HasValue);
			Assert.Equal(2, testScript.Value.Tests.Count());
			Assert.True(testScript.Value.TestSetup.HasValue);
			Assert.True(testScript.Value.TestCleanup.HasValue);
		}

		[Fact]
		public void Test_Discover_With_Multiple_Files()
		{
			// Arrange.
			var files = new[]
			{
				ScriptFiles.HasOneTest, 
				ScriptFiles.HasSetupAndCleanup,
				ScriptFiles.HasMultipleTests
			};
			

			// Act.
			var scripts = discoverer.Discover(files).ToList();

			// Assert.
			Assert.Equal(3, scripts.Count);

			Assert.Equal(1, scripts[0].Tests.Count());
			Assert.Equal(2, scripts[1].Tests.Count());
			Assert.Equal(4, scripts[2].Tests.Count());
		}

		private readonly PowerShellTestDiscoverer discoverer;

 		private readonly Mock<ILogger> logger = new Mock<ILogger>();
	}
}