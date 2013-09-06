using System;

namespace PSycheTest.Runners.Framework.Results
{
	/// <summary>
	/// Represents a PowerShell script error.
	/// </summary>
	public interface IErrorRecord
	{
		/// <summary>
		/// The script stack trace for the error.
		/// </summary>
		string ScriptStackTrace { get; }

		/// <summary>
		/// The exception that is associated with this error record.
		/// </summary>
		Exception Exception { get; }
	}
}