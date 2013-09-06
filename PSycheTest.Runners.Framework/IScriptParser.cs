using System.IO;

namespace PSycheTest.Runners.Framework
{
	/// <summary>
	/// Interface for a parser that creates an abstract syntax tree from a script file.
	/// </summary>
	internal interface IScriptParser
	{
		/// <summary>
		/// Parses the contents of a string.
		/// </summary>
		/// <param name="script">A string containing script code</param>
		/// <returns>An object containing an abstract syntax tree and any errors</returns>
		ParseResult Parse(string script);

		/// <summary>
		/// Parses the contents of a file.
		/// </summary>
		/// <param name="scriptFile">A file containing script code</param>
		/// <returns>An object containing an abstract syntax tree and any errors</returns>
		ParseResult Parse(FileInfo scriptFile);
	}
}