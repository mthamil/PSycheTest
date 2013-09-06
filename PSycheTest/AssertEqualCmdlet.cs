using System.Management.Automation;
using PSycheTest.Exceptions;

namespace PSycheTest
{
	/// <summary>
	/// Cmdlet that verifies that an expected value matches an actual value.
	/// </summary>
	[Cmdlet(VerbsLifecycle.Assert, "Equal")]
	public class AssertEqualCmdlet : AssertionCmdletBase
	{
		/// <summary>
		/// The expected value.
		/// </summary>
		[Parameter(Mandatory = true, Position = 0)]
		public object Expected { get; set; }

		/// <summary>
		/// The actual value.
		/// </summary>
		[Parameter(Mandatory = true, Position = 1)]
		public object Actual { get; set; }

		/// <see cref="AssertionCmdletBase.Assert"/>
		protected override void Assert()
		{
			if (!Equals(Expected, Actual))
				throw new ExpectedActualException(Expected, Actual, Message ?? "Assert-Equal Failure");
		}
	}
}