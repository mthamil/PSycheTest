using PSycheTest.Runners.Framework.Results;

namespace PSycheTest.Runners.Framework
{
	/// <summary>
	/// Represents information about a test function.
	/// </summary>
	public interface ITestFunction : ITestResultProvider
	{
		/// <summary>
		/// A test's full, unique name.
		/// </summary>
		string UniqueName { get; }

		/// <summary>
		/// A test's display name.
		/// </summary>
		string DisplayName { get; }

		/// <summary>
		/// A test function's programmatic name. This may or may not be the same as <see cref="UniqueName"/>.
		/// </summary>
		string FunctionName { get; }

		/// <summary>
		/// An optional test title.
		/// </summary>
		string CustomTitle { get; }

		/// <summary>
		/// Whether a test should be skipped.
		/// </summary>
		bool ShouldSkip { get; }

		/// <summary>
		/// If <see cref="ShouldSkip"/> is true, this should be the reason
		/// for skipping a test.
		/// </summary>
		string SkipReason { get; }

		/// <summary>
		/// Contains information about where a test came from.
		/// </summary>
		TestSourceInfo Source { get; }

		/// <summary>
		/// Adds a test result to a test.
		/// </summary>
		/// <param name="result">The result to add</param>
		void AddResult(TestResult result);
	}
}