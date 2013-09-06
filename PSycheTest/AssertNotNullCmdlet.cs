using System.Management.Automation;
using PSycheTest.Exceptions;

namespace PSycheTest
{
	/// <summary>
	/// Cmdlet that verifies that a value is not null.
	/// </summary>
	[Cmdlet(VerbsLifecycle.Assert, "NotNull")]
	public class AssertNotNullCmdlet : AssertionCmdletBase
	{
		/// <summary>
		/// The value that should not be null.
		/// </summary>
		[Parameter(Mandatory = true, Position = 0)]
		[AllowNull]
		public object Value { get; set; }

		/// <see cref="AssertionCmdletBase.Assert"/>
		protected override void Assert()
		{
			if (Value == null)
				throw new AssertionException(Message ?? "Assert-NotNull Failure");
		}
	}
}