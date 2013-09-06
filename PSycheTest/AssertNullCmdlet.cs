using System.Management.Automation;
using PSycheTest.Exceptions;

namespace PSycheTest
{
	/// <summary>
	/// Cmdlet that verifies that a value is null.
	/// </summary>
	[Cmdlet(VerbsLifecycle.Assert, "Null")]
	public class AssertNullCmdlet : AssertionCmdletBase
	{
		/// <summary>
		/// The actual value.
		/// </summary>
		[Parameter(Mandatory = true, Position = 0)]
		[AllowNull]
		public object Actual { get; set; }

		/// <see cref="AssertionCmdletBase.Assert"/>
		protected override void Assert()
		{
			if (Actual != null)
				throw new ExpectedActualException(null, Actual, Message ?? "Assert-Null Failure");
		}
	}
}