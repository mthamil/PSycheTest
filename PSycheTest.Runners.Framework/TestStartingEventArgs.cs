using System;

namespace PSycheTest.Runners.Framework
{
	/// <summary>
	/// Contains diagnostic information about when a test is about to start.
	/// </summary>
	public class TestStartingEventArgs : EventArgs
	{
		/// <summary>
		/// Initializes a new instance of <see cref="TestStartingEventArgs"/>.
		/// </summary>
		/// <param name="test">The test that is starting</param>
		public TestStartingEventArgs(ITestFunction test)
		{
			Test = test;
		}

		/// <summary>
		/// The test that is starting.
		/// </summary>
		public ITestFunction Test { get; private set; }
	}
}