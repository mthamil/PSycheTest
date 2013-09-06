using System.Management.Automation;
using PSycheTest.Exceptions;

namespace PSycheTest
{
	/// <summary>
	/// Cmdlet that verifies that a statement is true.
	/// </summary>
	[Cmdlet(VerbsLifecycle.Assert, "True")]
	public class AssertTrueCmdlet : AssertConditionCmdlet
	{
		/// <see cref="AssertionCmdletBase.Assert"/>
		protected override void Assert()
		{
			if (!Condition)
				throw new ExpectedActualException(true, false, Message ?? "Assert-True Failure");
		}
	}
}