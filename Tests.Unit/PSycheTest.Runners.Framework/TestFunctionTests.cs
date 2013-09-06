using System.IO;
using System.Management.Automation.Language;
using PSycheTest;
using PSycheTest.Runners.Framework;
using Xunit;

namespace Tests.Unit.PSycheTest.Runners.Framework
{
	public class TestFunctionTests
	{
		public TestFunctionTests()
		{
			function = SyntaxTree.OfFunctionNamed("Test-Function", fromFile: @".\Script.ps1", attribute: SyntaxTree.OfAttribute<TestAttribute>());
		}

		[Fact]
		public void Test_Properties()
		{
			// Act.
			var test = new TestFunction(function);

			// Assert.
			Assert.Equal(@".\Script.ps1:Test-Function", test.UniqueName);
			Assert.Equal("Test-Function", test.DisplayName);
			Assert.Equal(Path.GetFullPath(@".\Script.ps1"), test.Source.File.FullName);
			Assert.False(test.ShouldSkip);
			Assert.Equal(string.Empty, test.SkipReason);
		}

		private readonly FunctionDefinitionAst function;
	}
}