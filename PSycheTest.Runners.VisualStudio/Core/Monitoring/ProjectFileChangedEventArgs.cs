using System;
using Microsoft.VisualStudio.Shell.Interop;

namespace PSycheTest.Runners.VisualStudio.Core.Monitoring
{
	/// <summary>
	/// Contains information about file changes.
	/// </summary>
	public sealed class ProjectFileChangedEventArgs : EventArgs
	{
		/// <summary>
		/// Initializes a new instance of <see cref="ProjectFileChangedEventArgs"/>.
		/// </summary>
		/// <param name="project">The file's parent project</param>
		/// <param name="filePath">The path of the affected file</param>
		/// <param name="changeType">The type of change</param>
		public ProjectFileChangedEventArgs(IVsProject project, string filePath, FileChangeType changeType)
		{
			Project = project;
			FilePath = filePath;
			ChangeType = changeType;
		}

		/// <summary>
		/// The file's parent project.
		/// </summary>
		public IVsProject Project { get; private set; }

		/// <summary>
		/// The path of the affected file.
		/// </summary>
		public string FilePath { get; private set; }

		/// <summary>
		/// The type of change.
		/// </summary>
		public FileChangeType ChangeType { get; private set; }
	}
}