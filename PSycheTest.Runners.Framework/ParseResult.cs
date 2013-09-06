using System.Collections.Generic;
using System.Management.Automation.Language;

namespace PSycheTest.Runners.Framework
{
	/// <summary>
	/// Contains the results of a parse operation.
	/// </summary>
	internal class ParseResult
	{
		/// <summary>
		/// Initializes a new <see cref="ParseResult"/>.
		/// </summary>
		/// <param name="syntaxTree">The resulting abstract syntax tree</param>
		/// <param name="tokens">Parsed tokens</param>
		/// <param name="errors">Any parsing errors</param>
		public ParseResult(ScriptBlockAst syntaxTree, IEnumerable<Token> tokens, IEnumerable<ParseError> errors)
		{
			SyntaxTree = syntaxTree;
			Tokens = tokens;
			Errors = errors;
		}

		/// <summary>
		/// The resulting abstract syntax tree.
		/// </summary>
		public ScriptBlockAst SyntaxTree { get; private set; }

		/// <summary>
		/// Parsed tokens.
		/// </summary>
		public IEnumerable<Token> Tokens { get; private set; }

		/// <summary>
		/// Any parsing errors.
		/// </summary>
		public IEnumerable<ParseError> Errors { get; private set; }
	}
}