using System.Collections.Generic;
using PSycheTest.Runners.Framework.Results;

namespace PSycheTest.Runners.Framework
{
	/// <summary>
	/// Interface for an object that provides test results.
	/// </summary>
	public interface ITestResultProvider
	{
		/// <summary>
		/// Gets any test results.
		/// </summary>
		IEnumerable<TestResult> Results { get; }
	}
}