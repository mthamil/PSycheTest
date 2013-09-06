namespace PSycheTest.Runners.VisualStudio.Core.Monitoring
{
	/// <summary>
	/// Contains possible Visual Studio solution changes.
	/// </summary>
	public enum SolutionChangeType
	{
		/// <summary>
		/// Indicates a solution was loaded.
		/// </summary>
		Loaded,

		/// <summary>
		/// Indicates a solution was unloaded.
		/// </summary>
		Unloaded
	}
}