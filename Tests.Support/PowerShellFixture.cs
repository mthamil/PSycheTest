using System;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using PSycheTest.Runners.Framework.Extensions;
using PSycheTest.Runners.Framework.Utilities.Collections;

namespace Tests.Support
{
	/// <summary>
	/// Allows use of the same <see cref="PowerShell"/> for a fixture.
	/// </summary>
	public class PowerShellFixture : IDisposable
	{
		/// <summary>
		/// Adds cmdlets for the given implementing types to the current 
		/// <see cref="System.Management.Automation.Runspaces.RunspaceConfiguration"/>.
		/// </summary>
		public void AddCmdlets(Type firstCmdlet, params Type[] otherCmdlets)
		{
			var cmdlets = firstCmdlet.ToEnumerable().Concat(otherCmdlets);
			var currentRunspaceConfig = Shell.Runspace.RunspaceConfiguration;
			currentRunspaceConfig.Cmdlets
				.Append(cmdlets
					.Select(t => t.FromCmdletType())
					.Select(c => new CmdletConfigurationEntry(c.Name, c.ImplementingType, c.HelpFileName)));
			currentRunspaceConfig.Cmdlets.Update();
		}

		/// <summary>
		/// Accesses the current <see cref="PowerShell"/>.
		/// </summary>
		public PowerShell Shell { get { return _powershell.Value; } }

		private readonly Lazy<PowerShell> _powershell = 
			new Lazy<PowerShell>(() => PowerShell.Create(RunspaceMode.NewRunspace));

		public void Dispose()
		{
			Shell.Dispose();
		}
	}
}