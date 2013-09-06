using System.IO;
using System.Management.Automation;

namespace PSycheTest.Runners.Framework
{
	/// <summary>
	/// Class that manages and resets the appropriate PowerShell execution state between individual tests.
	/// </summary>
	public class TestExecutionTransaction: ITestExecutionTransaction
	{
		/// <summary>
		/// Initializes a new <see cref="TestExecutionTransaction"/>.
		/// </summary>
		/// <param name="powershell">The <see cref="PowerShell"/> to manage</param>
		/// <param name="locationManager">Manages a test's output</param>
		public TestExecutionTransaction(PowerShell powershell, ILocationManager locationManager)
		{
			_powershell = powershell;
			_locationManager = locationManager;

			OutputDirectory = locationManager.OutputDirectory;
			powershell.Runspace.SessionStateProxy.Path.SetLocation(_locationManager.ScriptLocation.FullName);
		}

		/// <see cref="ITestExecutionTransaction.OutputDirectory"/>
		public DirectoryInfo OutputDirectory { get; private set; }

		/// <summary>
		/// Cleans up after a test, including clearing errors, etc.
		/// </summary>
		public void Dispose()
		{
			_powershell.Commands.Clear();
			_powershell.Streams.ClearStreams();

			_locationManager.Dispose();

			// ResetRunspace resets the global variable table back to the initial state for that runspace 
			// (as well as cleaning up a few other things). What it doesn't do is clean out function definitions, 
			// types and format files or unload modules. This allows the API to be much faster. Also note that the 
			// Runspace has to have been created using an InitialSessionState object, not a RunspaceConfiguration 
			// instance.
			// http://stackoverflow.com/questions/13096061/call-a-powershell-script-in-a-new-clean-powershell-instance-from-within-anothe
			_powershell.Runspace.ResetRunspaceState();
		}

		private readonly ILocationManager _locationManager;
		private readonly PowerShell _powershell;
	}
}