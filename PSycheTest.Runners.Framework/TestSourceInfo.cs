using System.IO;

namespace PSycheTest.Runners.Framework
{
	/// <summary>
	/// Represents where a test came from.
	/// </summary>
	public class TestSourceInfo
	{
		/// <summary>
		/// Initializes a new instance of <see cref="TestSourceInfo"/>.
		/// </summary>
		public TestSourceInfo(FileInfo sourceFile, int lineNumber, int columnNumber)
		{
			File = sourceFile;
			LineNumber = lineNumber;
			ColumnNumber = columnNumber;
		}

		/// <summary>
		/// The file containing a test.
		/// </summary>
		public FileInfo File { get; private set; }

		/// <summary>
		/// The line in a file where a test begins.
		/// </summary>
		public int LineNumber { get; private set; }

		/// <summary>
		/// The column in a filer where a test begins.
		/// </summary>
		public int ColumnNumber { get; private set; }
	}
}