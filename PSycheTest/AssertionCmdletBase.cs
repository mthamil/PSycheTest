using System.Management.Automation;

namespace PSycheTest
{
	/// <summary>
	/// Base class for assertion cmdlets.
	/// </summary>
	public abstract class AssertionCmdletBase : PSCmdlet
	{
		/// <summary>
		/// An optional user message.
		/// </summary>
		[Parameter(Mandatory = false)]
		public string Message { get; set; }

		/// <see cref="Cmdlet.ProcessRecord"/>
		protected override void ProcessRecord()
		{
			Assert();
		}

		/// <summary>
		/// Executes a cmdlet's assertion.
		/// </summary>
		protected abstract void Assert();
	}
}