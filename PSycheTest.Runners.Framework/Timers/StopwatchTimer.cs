using System;
using System.Diagnostics;

namespace PSycheTest.Runners.Framework.Timers
{
	/// <summary>
	/// A test timer based on <see cref="Stopwatch"/>.
	/// </summary>
	internal class StopwatchTimer : ITestTimer
	{
		/// <see cref="ITestTimer.Start"/>
		public IDisposable Start()
		{
			stopwatch.Start();
			return new TimerToken(this);
		}

		/// <see cref="ITestTimer.Stop"/>
		public void Stop()
		{
			stopwatch.Stop();
			Elapsed = stopwatch.Elapsed;
		}

		/// <see cref="ITestTimer.Elapsed"/>
		public TimeSpan Elapsed { get; private set; }

		private readonly Stopwatch stopwatch = new Stopwatch();
	}
}