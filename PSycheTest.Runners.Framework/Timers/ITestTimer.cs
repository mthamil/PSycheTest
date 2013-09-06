using System;

namespace PSycheTest.Runners.Framework.Timers
{
	/// <summary>
	/// Interface for a timer for tests.
	/// </summary>
	public interface ITestTimer
	{
		/// <summary>
		/// Starts a timer and returns a token that will 
		/// automatically stop the timer when disposed.
		/// </summary>
		IDisposable Start();

		/// <summary>
		/// Stops a timer.
		/// </summary>
		void Stop();

		/// <summary>
		/// The amount of time elapsed.
		/// </summary>
		TimeSpan Elapsed { get; }
	}
}