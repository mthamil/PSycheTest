using System;

namespace PSycheTest.Runners.VisualStudio.Core.Monitoring
{
	/// <summary>
	/// Interface for an object that monitors a Visual Studio solution for changes.
	/// </summary>
	public interface ISolutionMonitor
	{
		/// <summary>
		/// Event raised when a solution is changed.
		/// </summary>
		event EventHandler<SolutionChangedEventArgs> SolutionChanged;

		/// <summary>
		/// Event raised when a project is changed.
		/// </summary>
		event EventHandler<ProjectChangedEventArgs> ProjectChanged;

		/// <summary>
		/// Begins monitoring a solution for changes.
		/// </summary>
		void StartMonitoring();

		/// <summary>
		/// Stops monitoring a solution for changes.
		/// </summary>
		void StopMonitoring();
	}
}