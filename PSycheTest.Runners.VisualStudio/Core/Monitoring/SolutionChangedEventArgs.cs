using System;
using Microsoft.VisualStudio.Shell.Interop;

namespace PSycheTest.Runners.VisualStudio.Core.Monitoring
{
	/// <summary>
	/// Contains information about solution changes.
	/// </summary>
	public sealed class SolutionChangedEventArgs : EventArgs
	{
		/// <summary>
		/// Initializes new event args.
		/// </summary>
		/// <param name="solution">The parent solution</param>
		/// <param name="changeType">The type of change</param>
		public SolutionChangedEventArgs(IVsSolution solution, SolutionChangeType changeType)
		{
			Solution = solution;
			ChangeType = changeType;
		}

		/// <summary>
		/// The parent solution.
		/// </summary>
		public IVsSolution Solution { get; private set; }

		/// <summary>
		/// The type of change.
		/// </summary>
		public SolutionChangeType ChangeType { get; private set; }
	}
}