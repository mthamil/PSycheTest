using System.IO;
using System.Management.Automation.Language;

namespace PSycheTest.Runners.Framework
{
	/// <summary>
	/// An <see cref="IScriptParser"/> that uses <see cref="System.Management.Automation.Language.Parser"/>.
	/// </summary>
	internal class PSScriptParser : IScriptParser
	{
		/// <see cref="IScriptParser.Parse(string)"/>
		public ParseResult Parse(string script)
		{
			Token[] tokens;
			ParseError[] errors;
			var ast = Parser.ParseInput(script, out tokens, out errors);
			return new ParseResult(ast, tokens, errors);
		}

		/// <see cref="IScriptParser.Parse(FileInfo)"/>
		public ParseResult Parse(FileInfo scriptFile)
		{
			Token[] tokens;
			ParseError[] errors;
			var ast = Parser.ParseFile(scriptFile.FullName, out tokens, out errors);
			return new ParseResult(ast, tokens, errors);
		}
	}
}