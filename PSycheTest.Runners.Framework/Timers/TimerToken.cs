using System;

namespace PSycheTest.Runners.Framework.Timers
{
	internal class TimerToken : IDisposable
	{
		/// <summary>
		/// Initializes a new <see cref="TimerToken"/>.
		/// </summary>
		/// <param name="timer">The <see cref="ITestTimer"/> to manage</param>
		public TimerToken(ITestTimer timer)
		{
			_timer = timer;
		}

		/// <see cref="IDisposable.Dispose"/>
		public void Dispose()
		{
			_timer.Stop();
		}

		private readonly ITestTimer _timer;
	}
}