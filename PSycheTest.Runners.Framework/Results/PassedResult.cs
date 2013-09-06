using System;
using System.Collections.Generic;

namespace PSycheTest.Runners.Framework.Results
{
	/// <summary>
	/// Represents the result of a test that passed.
	/// </summary>
	public class PassedResult : TestResult
	{
		/// <summary>
		/// Initializes a new <see cref="PassedResult"/>.
		/// </summary>
		/// <param name="duration">The time it took a test to execute</param>
		/// <param name="artifacts">Any artifacts created during execution</param>
		public PassedResult(TimeSpan duration, IEnumerable<Uri> artifacts)
			: base(duration, artifacts)
		{
		}

		/// <see cref="TestResult.Status"/>
		public override TestStatus Status
		{
			get { return TestStatus.Passed; }
		}
	}
}