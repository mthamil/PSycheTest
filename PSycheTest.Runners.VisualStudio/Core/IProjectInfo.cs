using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.Shell.Interop;

namespace PSycheTest.Runners.VisualStudio.Core
{
	/// <summary>
	/// Provides information about an <see cref="IVsProject"/>.
	/// </summary>
	public interface IProjectInfo
	{
		/// <summary>
		/// A project's name.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// A project's file.
		/// </summary>
		FileInfo File { get; }

		/// <summary>
		/// Whether a project corresponds to an actual project file or
		/// instead represents solution files or currently open
		/// files that do not belong to a solution (ie. "Miscellaneous Files").
		/// </summary>
		bool IsPhysicalProject { get; }

		/// <summary>
		/// Iterates over the child items of an <see cref="IVsProject"/>.
		/// </summary>
		IEnumerable<string> GetProjectItems();
	}
}