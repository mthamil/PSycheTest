using System;
using PSycheTest.Runners.Framework.Results;

namespace PSycheTest.Runners.Framework
{
	/// <summary>
	/// Contains diagnostic information about when a test has ended.
	/// </summary>
	public class TestEndedEventArgs : EventArgs
	{
		/// <summary>
		/// Initializes a new instance of <see cref="TestStartingEventArgs"/>.
		/// </summary>
		/// <param name="test">The test that has ended</param>
		/// <param name="result">The result of the test</param>
		public TestEndedEventArgs(ITestFunction test, TestResult result)
		{
			Test = test;
			Result = result;
		}

		/// <summary>
		/// The test that has ended.
		/// </summary>
		public ITestFunction Test { get; private set; }

		/// <summary>
		/// The result of the test.
		/// </summary>
		public TestResult Result { get; private set; }
	}
}