using System.Collections.Generic;

namespace PSycheTest.Runners.VisualStudio.Core
{
	/// <summary>
	/// Contains test run settings.
	/// </summary>
	public interface IPSycheTestRunSettings
	{
		/// <summary>
		/// Any modules that should be imported into the initial session state.
		/// </summary>
		ICollection<string> Modules { get; }
	}
}