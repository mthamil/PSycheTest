using System;
using Microsoft.VisualStudio.Shell.Interop;

namespace PSycheTest.Runners.VisualStudio.Core
{
	/// <summary>
	/// Contains extension methods for Visual Studio project hierarchy-related objects.
	/// </summary>
	public static class VisualStudioHierarchyExtensions
	{
		/// <summary>
		/// Provides access to additional information about a solution.
		/// </summary>
		/// <param name="solution">The solution to provide information about</param>
		/// <returns>An object with access to more solution data</returns>
		public static ISolutionInfo Info(this IVsSolution solution)
		{
			return SolutionInfoFactory(solution);
		}

		/// <summary>
		/// Provides access to additional information about a project.
		/// </summary>
		/// <param name="project">The project to provide information about</param>
		/// <returns>An object with access to more project data</returns>
		public static IProjectInfo Info(this IVsProject project)
		{
			return ProjectInfoFactory(project);
		}

		/// <summary>
		/// Factory for <see cref="ISolutionInfo"/>s.
		/// </summary>
		internal static Func<IVsSolution, ISolutionInfo> SolutionInfoFactory = solution => new SolutionInfo(solution);

		/// <summary>
		/// Factory for <see cref="IProjectInfo"/>s.
		/// </summary>
		internal static Func<IVsProject, IProjectInfo> ProjectInfoFactory = project => new ProjectInfo(project); 
	}
}