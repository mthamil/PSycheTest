using System;
using System.Linq;

namespace PSycheTest.Runners.Framework.Results
{
	/// <summary>
	/// Represents the result of a test that was skipped.
	/// </summary>
	public class SkippedResult : TestResult
	{
		/// <summary>
		/// Initializes a new <see cref="SkippedResult"/>.
		/// </summary>
		/// ><param name="skipReason">The reason a test was skipped.</param>
		public SkippedResult(string skipReason)
			: base(null, Enumerable.Empty<Uri>())
		{
			SkipReason = skipReason;
		}

		/// <see cref="TestResult.Status"/>
		public override TestStatus Status
		{
			get { return TestStatus.Skipped; }
		}

		/// <summary>
		/// The reason a test was skipped.
		/// </summary>
		public string SkipReason { get; private set; }
	}
}