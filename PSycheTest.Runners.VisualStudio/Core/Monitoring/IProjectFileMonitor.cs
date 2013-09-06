using System;

namespace PSycheTest.Runners.VisualStudio.Core.Monitoring
{
	/// <summary>
	/// Interface for an object that monitor project file changes.
	/// </summary>
	public interface IProjectFileMonitor
	{
		/// <summary>
		/// Event raised when a file is changed.
		/// </summary>
		event EventHandler<ProjectFileChangedEventArgs> FileChanged; 

		/// <summary>
		/// Begins monitoring a project for file changes.
		/// </summary>
		void StartMonitoring();

		/// <summary>
		/// Stops monitoring a project for file changes.
		/// </summary>
		void StopMonitoring();
	}
}