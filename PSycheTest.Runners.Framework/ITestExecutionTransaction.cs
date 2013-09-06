using System;
using System.IO;

namespace PSycheTest.Runners.Framework
{
	/// <summary>
	/// Interface for an object that manages and resets the appropriate PowerShell execution state between individual tests.
	/// </summary>
	public interface ITestExecutionTransaction : IDisposable
	{
		/// <summary>
		/// A test's output directory. If empty after execution of a test, it will be removed.
		/// </summary>
		DirectoryInfo OutputDirectory { get; }
	}
}