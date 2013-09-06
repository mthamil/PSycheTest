using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestWindow.Extensibility;

namespace PSycheTest.Runners.VisualStudio.Core
{
	/// <summary>
	/// Represents a Visual Studio test container.
	/// </summary>
	public interface IVSTestContainer : ITestContainer
	{
		/// <summary>
		/// A test container's parent project.
		/// </summary>
		IVsProject Project { get; }
	}
}