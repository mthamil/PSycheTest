using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using PSycheTest.Exceptions;
using PSycheTest.Runners.Framework;
using PSycheTest.Runners.Framework.Results;
using PSycheTest.Runners.Framework.Timers;
using PSycheTest.Runners.Framework.Utilities.Collections;
using Tests.Integration.TestScripts;
using Tests.Support;
using Xunit;

namespace Tests.Integration.PSycheTest.Runners.Framework
{
	public class PowerShellTestExecutorTests
	{
		public PowerShellTestExecutorTests()
		{
			discoverer = new PowerShellTestDiscoverer(logger.Object);
			executor = new PowerShellTestExecutor(logger.Object, () => new StopwatchTimer(), new SynchronousTaskScheduler())
			{
				OutputDirectory = new DirectoryInfo(Directory.GetCurrentDirectory())
			};
		}

		[Fact]
		public async Task Test_ExecuteAsync_Single_File_Single_Test()
		{
			// Arrange.
			var testScript = discoverer.Discover(ScriptFiles.HasOneTest);

			// Act.
			await executor.ExecuteAsync(testScript.Value.ToEnumerable());

			var results = testScript.Value.Results.ToList();

			// Assert.
			Assert.NotEmpty(results);
			Assert.Single(results);

			var result = Assert.IsType<FailedResult>(results.Single());
			Assert.Equal(TestStatus.Failed, result.Status);
			Assert.NotNull(result.Duration);
			Assert.True(result.Duration > TimeSpan.Zero);

			var reason = Assert.IsType<ExpectedActualException>(result.Reason.Exception);
			Assert.Equal(true, reason.Expected);
			Assert.Equal(false, reason.Actual);
		}

		[Fact]
		public async Task Test_ExecuteAsync_Single_File_Multiple_Tests()
		{
			// Arrange.
			var testScript = discoverer.Discover(ScriptFiles.HasMultipleTests);

			// Act.
			await executor.ExecuteAsync(testScript.Value.ToEnumerable());

			var results = testScript.Value.Results.ToList();

			// Assert.
			Assert.Equal(4, results.Count);

			var firstResult = Assert.IsType<FailedResult>(results[0]);
			Assert.Equal(TestStatus.Failed, firstResult.Status);
			Assert.NotNull(firstResult.Duration);
			Assert.True(firstResult.Duration > TimeSpan.Zero);
			Assert.IsType<ExpectedActualException>(firstResult.Reason.Exception);

			var secondResult = Assert.IsType<FailedResult>(results[1]);
			Assert.Equal(TestStatus.Failed, secondResult.Status);
			Assert.NotNull(secondResult.Duration);
			Assert.True(secondResult.Duration > TimeSpan.Zero);
			Assert.IsType<ExpectedActualException>(secondResult.Reason.Exception);

			var thirdResult = Assert.IsType<PassedResult>(results[2]);
			Assert.Equal(TestStatus.Passed, thirdResult.Status);
			Assert.NotNull(thirdResult.Duration);
			Assert.True(thirdResult.Duration > TimeSpan.Zero);

			var fourthResult = Assert.IsType<PassedResult>(results[3]);
			Assert.Equal(TestStatus.Passed, fourthResult.Status);
			Assert.NotNull(fourthResult.Duration);
			Assert.True(fourthResult.Duration > TimeSpan.Zero);
		}

		[Fact]
		public async Task Test_ExecuteAsync_Single_File_Skipped_Test()
		{
			// Arrange.
			var testScript = discoverer.Discover(ScriptFiles.HasSkippedTest);

			// Act.
			await executor.ExecuteAsync(testScript.Value.ToEnumerable());

			var results = testScript.Value.Results.ToList();

			// Assert.
			Assert.NotEmpty(results);
			Assert.Equal(2, results.Count);

			var skippedResult = Assert.IsType<SkippedResult>(results.First());
			Assert.Equal(TestStatus.Skipped, skippedResult.Status);
			Assert.Null(skippedResult.Duration);

			var passedResult = Assert.IsType<PassedResult>(results.Last());
			Assert.Equal(TestStatus.Passed, passedResult.Status);
			Assert.NotNull(passedResult.Duration);
			Assert.True(passedResult.Duration > TimeSpan.Zero);
		}

		[Fact]
		public async Task Test_ExecuteAsync_Single_File_Setup_And_Cleanup()
		{
			// Arrange.
			var testScript = discoverer.Discover(ScriptFiles.HasSetupAndCleanup);

			// Act.
			await executor.ExecuteAsync(testScript.Value.ToEnumerable());

			var results = testScript.Value.Results.ToList();

			// Assert.
			Assert.NotEmpty(results);
			Assert.Equal(2, results.Count);

			foreach (var result in results)
			{
				Assert.IsType<PassedResult>(result);
				Assert.Equal(TestStatus.Passed, result.Status);
				Assert.NotNull(result.Duration);
				Assert.True(result.Duration > TimeSpan.Zero);
			}
		}

		[Fact]
		public async Task Test_ExecuteAsync_Multiple_Files()
		{
			// Arrange.
			var scripts = discoverer.Discover(new[]
			{
				ScriptFiles.HasSetupAndCleanup, ScriptFiles.HasMultipleTests
			}).ToList();

			// Act.
			await executor.ExecuteAsync(scripts);

			var results = scripts.SelectMany(s => s.Results).ToList();

			// Assert.
			Assert.Equal(6, results.Count);
			Assert.Equal(new[] { TestStatus.Passed, TestStatus.Passed, TestStatus.Failed, TestStatus.Failed, TestStatus.Passed, TestStatus.Passed },
				results.Select(r => r.Status));
		}

		[Fact]
		public async Task Test_ExecuteAsync_Tests_Failed_By_NonFunction_Error()
		{
			// Arrange.
			var testScript = discoverer.Discover(ScriptFiles.HasNonFunctionErrors);

			// Act.
			await executor.ExecuteAsync(testScript.Value.ToEnumerable());

			var results = testScript.Value.Results.ToList();

			// Assert.
			Assert.Equal(2, results.Count);

			var firstResult = Assert.IsType<FailedResult>(results[0]);
			Assert.Equal(TestStatus.Failed, firstResult.Status);
			Assert.NotNull(firstResult.Duration);
			Assert.True(firstResult.Duration > TimeSpan.Zero);
			Assert.IsType<ExpectedActualException>(firstResult.Reason.Exception);

			var secondResult = Assert.IsType<FailedResult>(results[1]);
			Assert.Equal(TestStatus.Failed, secondResult.Status);
			Assert.NotNull(secondResult.Duration);
			Assert.True(secondResult.Duration > TimeSpan.Zero);
			Assert.IsType<ExpectedActualException>(secondResult.Reason.Exception);
		}

		[Fact]
		public async Task Test_ExecuteAsync_With_Filter()
		{
			// Arrange.
			var testScript = discoverer.Discover(ScriptFiles.HasMultipleTests).Value;

			// Act.
			await executor.ExecuteAsync(testScript.ToEnumerable(), tf => !tf.FunctionName.EndsWith("Failure"));

			var results = testScript.Results.ToList();

			// Assert.
			Assert.Equal(2, results.Count);

			foreach (var result in results)
			{
				var passed = Assert.IsType<PassedResult>(result);
				Assert.Equal(TestStatus.Passed, passed.Status);
			}
		}

		[Fact]
		public async Task Test_ExecuteAsync_With_External_Script_Reference()
		{
			// Arrange.
			var testScript = discoverer.Discover(ScriptFiles.HasScriptReferences);

			// Act.
			await executor.ExecuteAsync(testScript.Value.ToEnumerable());

			var results = testScript.Value.Results.ToList();

			// Assert.
			Assert.NotEmpty(results);
			Assert.Single(results);

			var result = Assert.IsType<PassedResult>(results.Single());
			Assert.Equal(TestStatus.Passed, result.Status);
			Assert.NotNull(result.Duration);
			Assert.True(result.Duration > TimeSpan.Zero);
		}

		private readonly PowerShellTestExecutor executor;

		private readonly PowerShellTestDiscoverer discoverer;
		private readonly Mock<ILogger> logger = new Mock<ILogger>(); 
	}
}