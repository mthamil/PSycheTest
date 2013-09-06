using System;

namespace PSycheTest.Runners.Framework.Results
{
	/// <summary>
	/// Base class for information about an error that occurred while executing a test script.
	/// </summary>
	public abstract class ScriptError
	{
		/// <summary>
		/// A description of the error.
		/// </summary>
		public abstract string Message { get; }

		/// <summary>
		/// The script error stack trace.
		/// </summary>
		public virtual string StackTrace
		{
			get { return Exception.StackTrace ?? string.Empty; }	// Some internal PowerShell exceptions seem to have null stack traces.
		}

		/// <summary>
		/// An error's associated exception.
		/// </summary>
		public abstract Exception Exception { get; }

		/// <summary>
		/// Allows filtering an error's stack trace.
		/// </summary>
		/// <param name="stackTrace">The stack trace to filter</param>
		/// <returns>The filtered stack trace</returns>
		protected virtual string FilterStackTrace(string stackTrace)
		{
			return stackTrace;
		}
	}
}