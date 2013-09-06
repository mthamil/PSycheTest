using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.Shell.Interop;

namespace PSycheTest.Runners.VisualStudio.Core
{
	/// <summary>
	/// Provides information about an <see cref="IVsSolution"/>.
	/// </summary>
	public interface ISolutionInfo
	{
		/// <summary>
		/// Iterates over the projects in an <see cref="IVsSolution"/>.
		/// </summary>
		IEnumerable<IVsProject> GetProjects();

		/// <summary>
		/// Returns the directory a solution is in.
		/// </summary>
		DirectoryInfo GetDirectory();
	}
}