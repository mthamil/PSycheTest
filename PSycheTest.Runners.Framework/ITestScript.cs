using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation.Language;
using PSycheTest.Runners.Framework.Results;
using PSycheTest.Runners.Framework.Utilities;

namespace PSycheTest.Runners.Framework
{
	/// <summary>
	/// Defines a test script.
	/// </summary>
	public interface ITestScript : ITestResultProvider
	{
		/// <summary>
		/// The file a script came from.
		/// </summary>
		FileInfo Source { get; }

		/// <summary>
		/// The text content of a script.
		/// </summary>
		string Text { get; }

		/// <summary>
		/// The tests contained with a script.
		/// </summary>
		IEnumerable<ITestFunction> Tests { get; }

		/// <summary>
		/// An optional test setup function.
		/// </summary>
		Option<FunctionDefinitionAst> TestSetup { get; }

		/// <summary>
		/// An optional test cleanup function.
		/// </summary>
		Option<FunctionDefinitionAst> TestCleanup { get; }

		/// <summary>
		/// Returns a script's results grouped by their parent <see cref="TestFunction"/>s.
		/// </summary>
		ILookup<ITestFunction, TestResult> ResultsByTest { get; }
	}
}