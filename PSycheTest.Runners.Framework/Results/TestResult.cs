using System;
using System.Collections.Generic;

namespace PSycheTest.Runners.Framework.Results
{
	/// <summary>
	/// Represents a test result.
	/// </summary>
	public abstract class TestResult
	{
		/// <summary>
		/// Initializes a new <see cref="TestResult"/>.
		/// </summary>
		/// <param name="duration">The time it took a test to execute</param>
		/// <param name="artifacts">Any artifacts created during execution</param>
		protected TestResult(TimeSpan? duration, IEnumerable<Uri> artifacts)
		{
			Duration = duration;
			Artifacts = new List<Uri>(artifacts);
		}

		/// <summary>
		/// A value representing a result's state.
		/// </summary>
		public abstract TestStatus Status { get; }

		/// <summary>
		/// The time it took a test to execute.
		/// </summary>
		public TimeSpan? Duration { get; private set; }

		/// <summary>
		/// Any artifacts from test execution.
		/// </summary>
		public IEnumerable<Uri> Artifacts { get; private set; }
	}
}