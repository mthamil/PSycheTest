using System.Collections.Generic;
using System.IO;
using PSycheTest.Runners.Framework.Utilities;

namespace PSycheTest.Runners.Framework
{
	/// <summary>
	/// Interface for an object that finds PowerShell tests.
	/// </summary>
	public interface IPowerShellTestDiscoverer
	{
		/// <summary>
		/// Finds test cases in a collection of script files.
		/// </summary>
		/// <param name="sourceFiles">The script files to search for test cases</param>
		/// <returns>Any discovered test cases</returns>
		IEnumerable<ITestScript> Discover(IEnumerable<FileInfo> sourceFiles);

		/// <summary>
		/// Finds test cases in a single file.
		/// </summary>
		/// <param name="sourceFile">The file to search for test cases</param>
		/// <returns>Any discovered test cases</returns>
		Option<ITestScript> Discover(FileInfo sourceFile);
	}
}