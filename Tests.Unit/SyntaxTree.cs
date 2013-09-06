using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation.Language;
using Moq;
using PSycheTest.Runners.Framework.Utilities.Collections;

namespace Tests.Unit
{
	/// <summary>
	/// Creates PowerShell syntax trees for testing purposes.
	/// </summary>
	public static class SyntaxTree
	{
		public static FunctionDefinitionAst OfFunctionNamed(string name, string fromFile = DefaultFileName, bool hasParamBlock = true, AttributeAst attribute = null, IScriptExtent extent = null)
		{
			var scriptExtent = extent ?? Extent(fromFile, 4, 5);
			var body = new ScriptBlockAst(scriptExtent,
					hasParamBlock
						? ParamBlock(scriptExtent, attribute: attribute)
						: null,
				StatementBlock(scriptExtent), false);

			return new FunctionDefinitionAst(scriptExtent, false, false, name, Enumerable.Empty<ParameterAst>(), body);
		}

		public static AttributeAst OfAttribute<TAttribute>(IEnumerable<ExpressionAst> positionalArguments = null, IEnumerable<NamedAttributeArgumentAst> namedArguments = null) 
			where TAttribute : Attribute
		{
			var typeName = Mock.Of<ITypeName>(t =>
			                                  t.GetReflectionAttributeType() == typeof(TAttribute) &&
			                                  t.Name == typeof(TAttribute).Name.Replace("Attribute", string.Empty) &&
			                                  t.FullName == typeof(TAttribute).FullName.Replace("Attribute", string.Empty));

			return new AttributeAst(Mock.Of<IScriptExtent>(),
			                 typeName,
							 positionalArguments ?? Enumerable.Empty<ExpressionAst>(),
							 namedArguments ?? Enumerable.Empty<NamedAttributeArgumentAst>());
		}

		public static ScriptBlockAst OfScript(string file = DefaultFileName, bool hasParamBlock = true, AttributeAst attribute = null, IEnumerable<StatementAst> statements = null)
		{
			var extent = Extent(file, 4, 5);
			return new ScriptBlockAst(extent, 
				hasParamBlock 
					? ParamBlock(extent, attribute: attribute)
 					: null,
				StatementBlock(extent, statements), false);
		}

		public static IScriptExtent Extent(string fileName = DefaultFileName, int startLine = 0, int startColumn = 0, string text = "")
		{
			return Mock.Of<IScriptExtent>(e =>
									   e.File == fileName &&
									   e.Text == text &&
									   e.StartLineNumber == startLine &&
									   e.StartColumnNumber == startColumn);
		}

		public static ParamBlockAst ParamBlock(IScriptExtent extent, IEnumerable<ParameterAst> parameters = null, AttributeAst attribute = null)
		{
			return new ParamBlockAst(extent,
						attribute == null ? Enumerable.Empty<AttributeAst>() : attribute.ToEnumerable(),
						parameters ?? Enumerable.Empty<ParameterAst>());
		}

		public static StatementBlockAst StatementBlock(IScriptExtent extent, IEnumerable<StatementAst> statements = null)
		{
			return new StatementBlockAst(extent, statements ?? Enumerable.Empty<StatementAst>(), Enumerable.Empty<TrapStatementAst>());
		}

		private const string DefaultFileName = @".\File.ps1";
	}
}