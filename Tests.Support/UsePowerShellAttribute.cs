using System;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Reflection;
using System.Threading;
using PSycheTest.Runners.Framework.Extensions;
using Xunit;

namespace Tests.Support
{
	/// <summary>
	/// Runs a test with a PowerShell environment running.
	/// </summary>
	public class UsePowerShellAttribute : BeforeAfterTestAttribute
	{
		/// <summary>
		/// Adds cmdlets for the given implementing types to the current runspace config.
		/// </summary>
		public Type[] WithCmdlets { get; set; }

		public override void Before(MethodInfo methodUnderTest)
		{
			_powershell = new ThreadLocal<PowerShell>(() => PowerShell.Create(RunspaceMode.NewRunspace));

			if (WithCmdlets != null)
			{
				var currentRunspaceConfig = _powershell.Value.Runspace.RunspaceConfiguration;
				currentRunspaceConfig.Cmdlets.Append(
					WithCmdlets
						.Select(t => CmdletExtensions.FromCmdletType(t))
						.Select(c => new CmdletConfigurationEntry(c.Name, c.ImplementingType, c.HelpFileName)));
				currentRunspaceConfig.Cmdlets.Update();
			}
		}

		public override void After(MethodInfo methodUnderTest)
		{
			_powershell.Value.Dispose();
			_powershell.Dispose();
		}

		/// <summary>
		/// The current <see cref="PowerShell"/> instance.
		/// </summary>
		public static PowerShell Shell
		{
			get { return _powershell.Value; }
		}

		private static ThreadLocal<PowerShell> _powershell;
	}
}