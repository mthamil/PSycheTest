using System;
using System.Management.Automation;

namespace PSycheTest.Runners.Framework.Results
{
	/// <summary>
	/// A wrapper around an <see cref="ErrorRecord"/>.
	/// </summary>
	public class ErrorRecordWrapper : IErrorRecord
	{
		/// <summary>
		/// Initializes a new <see cref="ErrorRecordWrapper"/>.
		/// </summary>
		/// <param name="innerError">The wrapped error</param>
		public ErrorRecordWrapper(ErrorRecord innerError)
		{
			_innerError = innerError;
		}

		/// <see cref="IErrorRecord.ScriptStackTrace"/>
		public string ScriptStackTrace { get { return _innerError.ScriptStackTrace; } }

		/// <see cref="IErrorRecord.Exception"/>
		public Exception Exception { get { return _innerError.Exception; } }

		private readonly ErrorRecord _innerError;
	}
}