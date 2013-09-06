using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation.Language;
using PSycheTest.Runners.Framework.Utilities;

namespace PSycheTest.Runners.Framework
{
	/// <summary>
	/// And <see cref="AstVisitor"/> that collects all test cases in a script.
	/// </summary>
	internal class TestDiscoveryVisitor : AstVisitor
	{
		/// <summary>
		/// Initializes a new <see cref="TestDiscoveryVisitor"/>.
		/// </summary>
		public TestDiscoveryVisitor()
		{
			TestSetupFunction = Option<FunctionDefinitionAst>.None();
			TestCleanupFunction = Option<FunctionDefinitionAst>.None();
		}

		/// <summary>
		/// Collected test functions.
		/// </summary>
		public IEnumerable<FunctionDefinitionAst> TestFunctions { get { return _testFunctions; } }

		/// <summary>
		/// An individual test setup function.
		/// </summary>
		public Option<FunctionDefinitionAst> TestSetupFunction { get; private set; }

		/// <summary>
		/// An individual test cleanup function.
		/// </summary>
		public Option<FunctionDefinitionAst> TestCleanupFunction { get; private set; }

		/// <see cref="AstVisitor.VisitFunctionDefinition"/>
		public override AstVisitAction VisitFunctionDefinition(FunctionDefinitionAst function)
		{
			if (HasAttribute<TestAttribute>(function))
			{
				_testFunctions.Add(function);
				return AstVisitAction.SkipChildren;
			}

			if (HasAttribute<TestSetupAttribute>(function))
			{
				TestSetupFunction = function;
				return AstVisitAction.SkipChildren;
			}

			if (HasAttribute<TestCleanupAttribute>(function))
			{
				TestCleanupFunction = function;
				return AstVisitAction.SkipChildren;
			}

			return AstVisitAction.Continue;
		}

		private static bool HasAttribute<TAttribute>(FunctionDefinitionAst function)
			where TAttribute : Attribute
		{
			if (ReferenceEquals(function.Body.ParamBlock, null))
				return false;

			var attributeAsts = function.Body.ParamBlock.Attributes;
			return attributeAsts.Any(AttributeMatches<TAttribute>);
		}

		private static bool AttributeMatches<TAttribute>(AttributeAst attributeAst)
			where TAttribute :Attribute
		{
			var attributeType = typeof(TAttribute);

			// A corresponding property must exist for each named argument.
			var matchingProperties = attributeAst.NamedArguments
				.Select(arg => attributeType.GetProperty(arg.ArgumentName, arg.Argument.StaticType));

			if (matchingProperties.Any(p => p == null))
				return false;

			// A constructor must exist with the number of position parameters given.
			var constructorArgs = attributeAst.PositionalArguments.Select(arg => arg.StaticType).ToArray();
			var matchingConstructor = attributeType.GetConstructor(constructorArgs);
			if (matchingConstructor == null)
				return false;

			return (attributeAst.TypeName.FullName + "Attribute").Equals(attributeType.FullName);
		}

		private readonly List<FunctionDefinitionAst> _testFunctions = new List<FunctionDefinitionAst>();
	}
}