using System;

namespace PSycheTest.Runners.Framework.Results
{
	/// <summary>
	/// Contains information about an exception-based script error.
	/// </summary>
	public class ExceptionScriptError : ScriptError
	{
		/// <summary>
		/// Initializes a new <see cref="ExceptionScriptError"/>.
		/// </summary>
		/// <param name="exception">The exception that caused the error</param>
		public ExceptionScriptError(Exception exception)
		{
			_exception = exception;
		}

		/// <see cref="ScriptError.StackTrace"/>
		public override string Message
		{
			get { return _exception.Message; }
		}

		/// <see cref="ScriptError.Exception"/>
		public override Exception Exception
		{
			get { return _exception; }
		}

		private readonly Exception _exception;
	}
}