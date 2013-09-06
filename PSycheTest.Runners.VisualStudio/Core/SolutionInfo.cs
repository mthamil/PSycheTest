using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace PSycheTest.Runners.VisualStudio.Core
{
	/// <summary>
	/// Provides information about an <see cref="IVsSolution"/>.
	/// </summary>
	internal class SolutionInfo : ISolutionInfo
	{
		/// <summary>
		/// Initializes a new <see cref="SolutionInfo"/>.
		/// </summary>
		/// <param name="solution">The solution to provide information about</param>
		public SolutionInfo(IVsSolution solution)
		{
			_solution = solution;
		}

		/// <see cref="ISolutionInfo.GetProjects"/>
		public IEnumerable<IVsProject> GetProjects()
		{
			return GetChildren(__VSENUMPROJFLAGS.EPF_LOADEDINSOLUTION).OfType<IVsProject>();
		}

		/// <summary>
		/// Iterates over the child items of an <see cref="IVsSolution"/>.
		/// </summary>
		/// <param name="enumFlags">Project options</param>
		private IEnumerable<IVsHierarchy> GetChildren(__VSENUMPROJFLAGS enumFlags)
		{
			var projectType = Guid.Empty;
			IEnumHierarchies hierarchyEnum;
			var result = _solution.GetProjectEnum((uint)enumFlags, ref projectType, out hierarchyEnum);

			if (ErrorHandler.Succeeded(result) && hierarchyEnum != null)
			{
				uint fetched;
				var hierarchies = new IVsHierarchy[1];
				while (hierarchyEnum.Next(1, hierarchies, out fetched) == VSConstants.S_OK)
				{
					yield return hierarchies[0];
				}
			}
		}

		/// <see cref="ISolutionInfo.GetDirectory"/>
		public DirectoryInfo GetDirectory()
		{
			string directory;
			string file;
			string userOptionsFile;
			int result = _solution.GetSolutionInfo(out directory, out file, out userOptionsFile);
			ErrorHandler.ThrowOnFailure(result);
			return new DirectoryInfo(directory);
		}

		private readonly IVsSolution _solution;
	}
}