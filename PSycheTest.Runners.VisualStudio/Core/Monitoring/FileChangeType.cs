namespace PSycheTest.Runners.VisualStudio.Core.Monitoring
{
	/// <summary>
	/// Contains possible file change types.
	/// </summary>
	public enum FileChangeType
	{
		/// <summary>
		/// Indicates that a file was added.
		/// </summary>
		Added,

		/// <summary>
		/// Indicates that a file was deleted.
		/// </summary>
		Removed,

		/// <summary>
		/// Indicates that a file was updated.
		/// </summary>
		Modified
	}
}