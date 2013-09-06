using System;

namespace PSycheTest.Runners.Framework
{
	/// <summary>
	/// Contains diagnostic information about when a test script is about to start.
	/// </summary>
	public class TestScriptStartingEventArgs : EventArgs
	{
		/// <summary>
		/// Initializes a new instance of <see cref="TestScriptStartingEventArgs"/>.
		/// </summary>
		/// <param name="script">The test script that is starting</param>
		public TestScriptStartingEventArgs(ITestScript script)
		{
			Script = script;
		}

		/// <summary>
		/// The test script that is starting.
		/// </summary>
		public ITestScript Script { get; private set; }
	}
}