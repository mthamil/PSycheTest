using System;
using System.Management.Automation.Language;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Moq;
using PSycheTest;
using PSycheTest.Runners.Framework;
using PSycheTest.Runners.Framework.Results;
using PSycheTest.Runners.Framework.Utilities.Collections;
using PSycheTest.Runners.VisualStudio;
using PSycheTest.Runners.VisualStudio.Core;
using Xunit;
using Xunit.Extensions;

namespace Tests.Unit.PSycheTest.Runners.VisualStudio.Core
{
	public class TestMapperTests
	{
		[Fact]
		public void Test_Map_TestFunction()
		{
			// Arrange.
			var extent = Mock.Of<IScriptExtent>(e =>
				e.File == @"C:\TestScript.ps1" &&
				e.StartLineNumber == 12 &&
				e.StartColumnNumber == 20);

			var testFunction = new TestFunction(SyntaxTree.OfFunctionNamed("Test-Function", 
				attribute: SyntaxTree.OfAttribute<TestAttribute>(), extent: extent));

			// Act.
			var testCase = mapper.Map(testFunction);

			// Assert.
			Assert.Equal("Test-Function", testCase.DisplayName);
			Assert.Equal(@"C:\TestScript.ps1:Test-Function", testCase.FullyQualifiedName);
			Assert.Equal(@"C:\TestScript.ps1", testCase.Source);
			Assert.Equal(@"C:\TestScript.ps1", testCase.CodeFilePath);
			Assert.Equal(12, testCase.LineNumber);
			Assert.Equal(VSTestExecutor.ExecutorUri, testCase.ExecutorUri);
		}

		[Theory]
		[InlineData(TestOutcome.None, TestStatus.NotExecuted)]
		[InlineData(TestOutcome.Passed, TestStatus.Passed)]
		[InlineData(TestOutcome.Failed, TestStatus.Failed)]
		[InlineData(TestOutcome.Skipped, TestStatus.Skipped)]
		public void Test_Map_TestStatus(TestOutcome expected, TestStatus status)
		{
			// Act.
			var actual = mapper.Map(status);

			// Assert.
			Assert.Equal(expected, actual);
		}

		[Fact]
		public void Test_Map_PassedResult()
		{
			// Arrange.
			var testCase = new TestCase("This.Is.A.Test", new Uri("executor://The.Executor"), @"C:\TestScript.ps1");
			var result = new PassedResult(TimeSpan.FromSeconds(2), new Uri(@"C:\artifact.txt").ToEnumerable());

			// Act.
			var mappedResult = mapper.Map(testCase, result);

			// Assert.
			Assert.Equal(TestOutcome.Passed, mappedResult.Outcome);
			Assert.Equal(TimeSpan.FromSeconds(2), mappedResult.Duration);
			Assert.Equal(testCase, mappedResult.TestCase);
			Assert.Null(mappedResult.ErrorStackTrace);
			Assert.Null(mappedResult.ErrorMessage);

			var attachmentSet = Assert.Single(mappedResult.Attachments);
			var attachment = Assert.Single(attachmentSet.Attachments);
			Assert.Equal(@"C:\artifact.txt", attachment.Uri.OriginalString);
		}

		[Fact]
		public void Test_Map_FailedResult()
		{
			// Arrange.
			var testCase = new TestCase("This.Is.A.Test", new Uri("executor://The.Executor"), @"C:\TestScript.ps1");
			var exception = Record.Exception(() => { throw new InvalidOperationException("Error!"); });	// Populate the stack trace.

			var result = new FailedResult(TimeSpan.FromSeconds(3), new ExceptionScriptError(exception));

			// Act.
			var mappedResult = mapper.Map(testCase, result);

			// Assert.
			Assert.Equal(TestOutcome.Failed, mappedResult.Outcome);
			Assert.Equal(TimeSpan.FromSeconds(3), mappedResult.Duration);
			Assert.Equal(testCase, mappedResult.TestCase);
			Assert.Equal(exception.StackTrace, mappedResult.ErrorStackTrace);
			Assert.Equal(exception.Message, mappedResult.ErrorMessage);
		}

		[Fact]
		public void Test_Map_SkippedResult()
		{
			// Arrange.
			var testCase = new TestCase("This.Is.A.Test", new Uri("executor://The.Executor"), @"C:\TestScript.ps1");
			var result = new SkippedResult("Because!");

			// Act.
			var mappedResult = mapper.Map(testCase, result);

			// Assert.
			Assert.Equal(TestOutcome.Skipped, mappedResult.Outcome);
			Assert.Equal(TimeSpan.Zero, mappedResult.Duration);
			Assert.Equal(testCase, mappedResult.TestCase);
			Assert.Null(mappedResult.ErrorStackTrace);
			Assert.Null(mappedResult.ErrorMessage);
		}

 		private readonly TestMapper mapper = new TestMapper();
	}
}