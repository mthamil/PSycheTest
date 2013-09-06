using System.Management.Automation;
using PSycheTest.Exceptions;

namespace PSycheTest
{
	/// <summary>
	/// Cmdlet that verifies that a statement is false.
	/// </summary>
	[Cmdlet(VerbsLifecycle.Assert, "False")]
	public class AssertFalseCmdlet : AssertConditionCmdlet
	{
		/// <see cref="AssertionCmdletBase.Assert"/>
		protected override void Assert()
		{
			if (Condition)
				throw new ExpectedActualException(false, true, Message ?? "Assert-False Failure");
		}
	}
}