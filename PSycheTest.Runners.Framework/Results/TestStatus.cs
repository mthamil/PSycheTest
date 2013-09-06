namespace PSycheTest.Runners.Framework.Results
{
	/// <summary>
	/// The possible states of a test.
	/// </summary>
	public enum TestStatus
	{
		/// <summary>
		/// Indicates that a test has not been run.
		/// </summary>
		NotExecuted,

		/// <summary>
		/// Indicates that a test has completed successfully.
		/// </summary>
		Passed,

		/// <summary>
		/// Indicates that a test has failed.
		/// </summary>
		Failed,

		/// <summary>
		/// Indicates that a test was skipped.
		/// </summary>
		Skipped
	}
}