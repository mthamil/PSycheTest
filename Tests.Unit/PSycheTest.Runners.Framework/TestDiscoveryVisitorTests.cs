using System.Collections.Generic;
using System.Linq;
using System.Management.Automation.Language;
using Moq;
using PSycheTest;
using PSycheTest.Runners.Framework;
using Xunit;

namespace Tests.Unit.PSycheTest.Runners.Framework
{
	public class TestDiscoveryVisitorTests
	{
		[Fact]
		public void Test_Visit()
		{
			// Arrange.
			var functionSyntaxTree = SyntaxTree.OfFunctionNamed("Test-Function", 
				attribute: SyntaxTree.OfAttribute<TestAttribute>());

			// Act.
			functionSyntaxTree.Visit(visitor);

			// Assert.
			Assert.Single(visitor.TestFunctions);
			Assert.False(visitor.TestSetupFunction.HasValue);
			Assert.False(visitor.TestCleanupFunction.HasValue);

			var function = visitor.TestFunctions.Single();
			Assert.Equal("Test-Function", function.Name);

			var compiled = function.Body.GetScriptBlock();
			Assert.Single(compiled.Attributes.OfType<TestAttribute>());
		}

		[Fact]
		public void Test_Visit_With_Invalid_Attribute_Constructor_Arguments()
		{
			// Arrange.
			var positionalArgs = new List<ExpressionAst>
			{
				new ConstantExpressionAst(Mock.Of<IScriptExtent>(), "invalid")
			};

			var functionSyntaxTree = SyntaxTree.OfFunctionNamed("Test-Function",
				attribute: SyntaxTree.OfAttribute<TestAttribute>(positionalArgs));

			// Act.
			functionSyntaxTree.Visit(visitor);

			// Assert.
			Assert.Empty(visitor.TestFunctions);
		}

		[Fact]
		public void Test_Visit_With_Invalid_Attribute_Property_Count()
		{
			// Arrange.
			var extent = Mock.Of<IScriptExtent>();
			var namedArgs = new List<NamedAttributeArgumentAst>
			{
				new NamedAttributeArgumentAst(extent, "Description", new ConstantExpressionAst(extent, "descr"), false),
				new NamedAttributeArgumentAst(extent, "NonExistent", new ConstantExpressionAst(extent, "invalid"), false)
			};

			var functionSyntaxTree = SyntaxTree.OfFunctionNamed("Test-Function",
				attribute: SyntaxTree.OfAttribute<TestAttribute>(namedArguments:namedArgs));

			// Act.
			functionSyntaxTree.Visit(visitor);

			// Assert.
			Assert.Empty(visitor.TestFunctions);
		}

		[Fact]
		public void Test_Visit_With_Invalid_Attribute_Property_Type()
		{
			// Arrange.
			var extent = Mock.Of<IScriptExtent>();
			var namedArgs = new List<NamedAttributeArgumentAst>
			{
				new NamedAttributeArgumentAst(extent, "Description", new ConstantExpressionAst(extent, 1), false),
			};

			var functionSyntaxTree = SyntaxTree.OfFunctionNamed("Test-Function",
				attribute: SyntaxTree.OfAttribute<TestAttribute>(namedArguments: namedArgs));

			// Act.
			functionSyntaxTree.Visit(visitor);

			// Assert.
			Assert.Empty(visitor.TestFunctions);
		}

		[Fact]
		public void Test_Visit_With_No_Test_Attribute()
		{
			// Arrange.
			var functionSyntaxTree = SyntaxTree.OfFunctionNamed("Test-Function");

			// Act.
			functionSyntaxTree.Visit(visitor);

			// Assert.
			Assert.Empty(visitor.TestFunctions);
			Assert.False(visitor.TestSetupFunction.HasValue);
			Assert.False(visitor.TestCleanupFunction.HasValue);
		}

		[Fact]
		public void Test_Visit_With_No_Param_Block()
		{
			// Arrange.
			var functionSyntaxTree = SyntaxTree.OfFunctionNamed("Test-Function", hasParamBlock:false);

			// Act.
			functionSyntaxTree.Visit(visitor);

			// Assert.
			Assert.Empty(visitor.TestFunctions);
			Assert.False(visitor.TestSetupFunction.HasValue);
			Assert.False(visitor.TestCleanupFunction.HasValue);
		}

		[Fact]
		public void Test_Visit_Setup_Attribute()
		{
			// Arrange.
			var functionSyntaxTree = SyntaxTree.OfFunctionNamed("Setup-Function", 
				attribute: SyntaxTree.OfAttribute<TestSetupAttribute>());

			// Act.
			functionSyntaxTree.Visit(visitor);

			// Assert.
			Assert.True(visitor.TestSetupFunction.HasValue);
			Assert.False(visitor.TestCleanupFunction.HasValue);

			var setup = visitor.TestSetupFunction.Value;
			Assert.Equal("Setup-Function", setup.Name);

			var compiled = setup.Body.GetScriptBlock();
			Assert.Single(compiled.Attributes.OfType<TestSetupAttribute>());
		}

		[Fact]
		public void Test_Visit_Cleanup_Attribute()
		{
			// Arrange.
			var functionSyntaxTree = SyntaxTree.OfFunctionNamed("Cleanup-Function", 
				attribute: SyntaxTree.OfAttribute<TestCleanupAttribute>());

			// Act.
			functionSyntaxTree.Visit(visitor);

			// Assert.
			Assert.True(visitor.TestCleanupFunction.HasValue);
			Assert.False(visitor.TestSetupFunction.HasValue);

			var cleanup = visitor.TestCleanupFunction.Value;
			Assert.Equal("Cleanup-Function", cleanup.Name);

			var compiled = cleanup.Body.GetScriptBlock();
			Assert.Single(compiled.Attributes.OfType<TestCleanupAttribute>());
		}

		private readonly TestDiscoveryVisitor visitor = new TestDiscoveryVisitor();
	}
}