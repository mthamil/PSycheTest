using System.Collections.Generic;
using System.IO;
using System.Linq;
using PSycheTest.Runners.Framework.Utilities;

namespace PSycheTest.Runners.Framework
{
	/// <summary>
	/// A class that discovers tests based on script files.
	/// </summary>
	public class PowerShellTestDiscoverer : IPowerShellTestDiscoverer
	{
		/// <summary>
		/// Initializes a new <see cref="PowerShellTestDiscoverer"/>.
		/// </summary>
		/// <param name="logger">A message logger</param>
		public PowerShellTestDiscoverer(ILogger logger)
			: this(new PSScriptParser(), logger) { }

		/// <summary>
		/// Initializes a new <see cref="PowerShellTestDiscoverer"/>.
		/// </summary>
		/// <param name="parser">The parser to use for scripts</param>
		/// <param name="logger">A message logger</param>
		internal PowerShellTestDiscoverer(IScriptParser parser, ILogger logger)
		{
			_parser = parser;
			_logger = logger;
		}

		/// <summary>
		/// Finds test cases in a collection of script files.
		/// </summary>
		/// <param name="sourceFiles">The script files to search for test cases</param>
		/// <returns>Any discovered test cases</returns>
		public IEnumerable<ITestScript> Discover(IEnumerable<FileInfo> sourceFiles)
		{
			return sourceFiles.Select(Discover)
			                  .Where(script => script.HasValue)
			                  .Select(script => script.Value);
		}

		/// <summary>
		/// Finds test cases in a single file.
		/// </summary>
		/// <param name="sourceFile">The file to search for test cases</param>
		/// <returns>Any discovered test cases</returns>
		public Option<ITestScript> Discover(FileInfo sourceFile)
		{
			_logger.Info("Analyzing file '{0}' for tests.", sourceFile.FullName);

			var result = _parser.Parse(sourceFile);

			if (result.Errors.Any())
			{
				foreach (var error in result.Errors)
				{
					_logger.Error("Parsing error: {0} at Line: {1}, Column: {2}", error.Message, error.Extent.StartLineNumber, error.Extent.StartColumnNumber);
				}
				return Option<ITestScript>.None();
			}

			var visitor = new TestDiscoveryVisitor();
			result.SyntaxTree.Visit(visitor);

			var tests = visitor.TestFunctions.Select(function => new TestFunction(function)).ToList();
			if (!tests.Any())
				return Option<ITestScript>.None();

			return new TestScript(result.SyntaxTree, tests, visitor.TestSetupFunction, visitor.TestCleanupFunction);
		}

		private readonly IScriptParser _parser;
		private readonly ILogger _logger;
	}
}