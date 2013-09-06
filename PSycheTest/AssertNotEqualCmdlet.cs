using System.Management.Automation;
using PSycheTest.Exceptions;

namespace PSycheTest
{
	/// <summary>
	/// Cmdlet that verifies that an expected value matches an actual value.
	/// </summary>
	[Cmdlet(VerbsLifecycle.Assert, "NotEqual")]
	public class AssertNotEqualCmdlet : AssertionCmdletBase
	{
		/// <summary>
		/// The first value.
		/// </summary>
		[Parameter(Mandatory = true, Position = 0)]
		public object First { get; set; }

		/// <summary>
		/// The second value.
		/// </summary>
		[Parameter(Mandatory = true, Position = 1)]
		public object Second { get; set; }

		/// <see cref="AssertionCmdletBase.Assert"/>
		protected override void Assert()
		{
			if (Equals(First, Second))
				throw new AssertionException(Message ?? "Assert-NotEqual Failure");
		}
	}
}