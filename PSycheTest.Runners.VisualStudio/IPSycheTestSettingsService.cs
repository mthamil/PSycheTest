using PSycheTest.Runners.VisualStudio.Core;

namespace PSycheTest.Runners.VisualStudio
{
	/// <summary>
	/// Provides access to PSycheTest settings.
	/// </summary>
	public interface IPSycheTestSettingsService
	{
		/// <summary>
		/// The provided settings.
		/// </summary>
		IPSycheTestRunSettings Settings { get; }
	}
}