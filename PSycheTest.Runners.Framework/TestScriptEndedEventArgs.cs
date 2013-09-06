using System;

namespace PSycheTest.Runners.Framework
{
	/// <summary>
	/// Contains diagnostic information about when a test script has ended.
	/// </summary>
	public class TestScriptEndedEventArgs : EventArgs
	{
		/// <summary>
		/// Initializes a new instance of <see cref="TestScriptStartingEventArgs"/>.
		/// </summary>
		/// <param name="script">The test script that has ended</param>
		public TestScriptEndedEventArgs(ITestScript script)
		{
			Script = script;
		}

		/// <summary>
		/// The test script that has ended.
		/// </summary>
		public ITestScript Script { get; private set; }
	}
}