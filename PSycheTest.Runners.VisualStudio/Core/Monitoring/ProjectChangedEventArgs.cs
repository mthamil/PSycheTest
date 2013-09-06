using System;
using Microsoft.VisualStudio.Shell.Interop;

namespace PSycheTest.Runners.VisualStudio.Core.Monitoring
{
	/// <summary>
	/// Contains information about project changes.
	/// </summary>
	public sealed class ProjectChangedEventArgs : EventArgs
	{
		/// <summary>
		/// Initializes new event args.
		/// </summary>
		/// <param name="solution">The parent solution</param>
		/// <param name="project">The project that changed</param>
		/// <param name="changeType">The type of change</param>
		public ProjectChangedEventArgs(IVsSolution solution, IVsProject project, ProjectChangeType changeType)
		{
			Solution = solution;
			Project = project;
			ChangeType = changeType;
		}

		/// <summary>
		/// The parent solution.
		/// </summary>
		public IVsSolution Solution { get; private set; }

		/// <summary>
		/// The project that changed.
		/// </summary>
		public IVsProject Project { get; private set; }

		/// <summary>
		/// The type of change.
		/// </summary>
		public ProjectChangeType ChangeType { get; private set; }
	}
}