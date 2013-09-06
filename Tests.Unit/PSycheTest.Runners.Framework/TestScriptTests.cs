using System;
using System.Linq;
using System.Management.Automation.Language;
using PSycheTest;
using PSycheTest.Runners.Framework;
using PSycheTest.Runners.Framework.Results;
using PSycheTest.Runners.Framework.Utilities;
using Xunit;

namespace Tests.Unit.PSycheTest.Runners.Framework
{
	public class TestScriptTests
	{
		public TestScriptTests()
		{
			firstFunction = SyntaxTree.OfFunctionNamed("Test-First", attribute: SyntaxTree.OfAttribute<TestAttribute>());
			secondFunction = SyntaxTree.OfFunctionNamed("Test-Second", attribute: SyntaxTree.OfAttribute<TestAttribute>());
			scriptBlock = SyntaxTree.OfScript(statements: new[] { firstFunction, secondFunction });
		}

		[Fact]
		public void Test_Results()
		{
			// Arrange.
			var firstTest = new TestFunction(firstFunction);
			var secondTest = new TestFunction(secondFunction);
			var script = new TestScript(scriptBlock, new[] { firstTest, secondTest }, 
				Option<FunctionDefinitionAst>.None(), Option<FunctionDefinitionAst>.None());

			firstTest.AddResult(new PassedResult(TimeSpan.FromMilliseconds(200), Enumerable.Empty<Uri>()));
			firstTest.AddResult(new FailedResult(TimeSpan.FromMilliseconds(300), new ExceptionScriptError(new InvalidOperationException())));
			secondTest.AddResult(new SkippedResult("Skipped!"));

			// Act.
			var allResults = script.Results;

			// Assert.
			Assert.Equal(3, allResults.Count());
		}

		[Fact]
		public void Test_ResultsByTest()
		{
			// Arrange.
			var firstTest = new TestFunction(firstFunction);
			var secondTest = new TestFunction(secondFunction);
			var script = new TestScript(scriptBlock, new[] { firstTest, secondTest },
				Option<FunctionDefinitionAst>.None(), Option<FunctionDefinitionAst>.None());

			firstTest.AddResult(new PassedResult(TimeSpan.FromMilliseconds(200), Enumerable.Empty<Uri>()));
			firstTest.AddResult(new FailedResult(TimeSpan.FromMilliseconds(300), new ExceptionScriptError(new InvalidOperationException())));
			secondTest.AddResult(new SkippedResult("Skipped!"));

			// Act.
			var groupedResults = script.ResultsByTest;

			// Assert.
			Assert.Equal(2, groupedResults.Count);
			Assert.Equal(2, groupedResults[firstTest].Count());
			Assert.Equal(1, groupedResults[secondTest].Count());
		}

		private readonly FunctionDefinitionAst firstFunction;
		private readonly FunctionDefinitionAst secondFunction;
		private readonly ScriptBlockAst scriptBlock;
	}
}