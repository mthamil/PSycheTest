using System;
using System.IO;

namespace PSycheTest.Runners.Framework
{
	/// <summary>
	/// Interface for an object that manages the location of a test's output and current location.
	/// </summary>
	public interface ILocationManager : IDisposable
	{
		/// <summary>
		/// A test's output directory. If empty after execution of a test, it will be removed.
		/// </summary>
		DirectoryInfo OutputDirectory { get; }

		/// <summary>
		/// The actual location of an executing script.
		/// </summary>
		DirectoryInfo ScriptLocation { get; }
	}
}