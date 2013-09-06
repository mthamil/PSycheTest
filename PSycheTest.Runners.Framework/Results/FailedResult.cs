using System;
using System.Linq;

namespace PSycheTest.Runners.Framework.Results
{
	/// <summary>
	/// Represents the result of a test that failed.
	/// </summary>
	public class FailedResult : TestResult
	{
		/// <summary>
		/// Initializes a new <see cref="FailedResult"/>.
		/// </summary>
		/// <param name="duration">The time it took a test to execute</param>
		/// <param name="reason">The error that caused the failure</param>
		public FailedResult(TimeSpan duration, ScriptError reason)
			: base(duration, Enumerable.Empty<Uri>())
		{
			Reason = reason;
		}

		/// <see cref="TestResult.Status"/>
		public override TestStatus Status
		{
			get { return TestStatus.Failed; }
		}

		/// <summary>
		/// The error that caused the test failure.
		/// </summary>
		public ScriptError Reason { get; private set; }
	}
}