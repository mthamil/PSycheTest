using System;
using System.Management.Automation.Runspaces;
using System.Threading.Tasks;

namespace PSycheTest.Runners.Framework.Extensions
{
	/// <summary>
	/// Provides extension methods for <see cref="Runspace"/>s.
	/// </summary>
	public static class RunspaceExtensions
	{
		/// <summary>
		/// Opens a <see cref="Runspace"/> asynchronously.
		/// </summary>
		/// <param name="runspace">The runspace to open</param>
		/// <returns>A task that will complete when the runspace opens</returns>
		public static Task OpenTaskAsync(this Runspace runspace)
		{
			// If the runspace is already open, return immediately.
			if (runspace.RunspaceStateInfo.State == RunspaceState.Opened)
				return Task.FromResult<object>(null);

			var tcs = new TaskCompletionSource<object>();
			EventHandler<RunspaceStateEventArgs> stateHandler = null;
			stateHandler = (o, e) =>
			{
				if (e.RunspaceStateInfo.Reason != null)
				{
					runspace.StateChanged -= stateHandler;
					tcs.TrySetException(e.RunspaceStateInfo.Reason);
				}
				else if (e.RunspaceStateInfo.State == RunspaceState.Opened)
				{
					runspace.StateChanged -= stateHandler;
					tcs.TrySetResult(null);
				}
			};
			runspace.StateChanged += stateHandler;
			runspace.OpenAsync();

			return tcs.Task;
		}
	}
}