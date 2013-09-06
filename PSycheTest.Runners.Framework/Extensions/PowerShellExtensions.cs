using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;

namespace PSycheTest.Runners.Framework.Extensions
{
	/// <summary>
	/// Contains extension methods for <see cref="PowerShell"/>.
	/// </summary>
	public static class PowerShellExtensions
	{
		/// <summary>
		/// Invokes a PowerShell pipeline asynchronously
		/// </summary>
		/// <param name="powerShell">The <see cref="PowerShell"/> instance to invoke</param>
		/// <param name="taskScheduler">An optional <see cref="TaskScheduler"/></param>
		/// <returns>A <see cref="Task"/> representing the asynchronous invocation</returns>
		public static async Task<IReadOnlyCollection<ErrorRecord>> InvokeAsync(this PowerShell powerShell, TaskScheduler taskScheduler = null)
		{
			await Task.Factory.FromAsync(
				powerShell.BeginInvoke(), 
				r => powerShell.EndInvoke(r), 
				TaskCreationOptions.None, taskScheduler ?? TaskScheduler.Default).ConfigureAwait(false);

			return powerShell.Streams.Error.ToList();
		}
	}
}