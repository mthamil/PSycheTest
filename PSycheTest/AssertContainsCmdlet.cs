using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using PSycheTest.Exceptions;

namespace PSycheTest
{
	/// <summary>
	/// Cmdlet that verifies that a collection contains a given value.
	/// </summary>
	[Cmdlet(VerbsLifecycle.Assert, "Contains")]
	public class AssertContainsCmdlet : AssertionCmdletBase
	{
		/// <summary>
		/// The expected value.
		/// </summary>
		[Parameter(Mandatory = true, Position = 0)]
		public object Expected { get; set; }

		/// <summary>
		/// The collection to check.
		/// </summary>
		[Parameter(Mandatory = true, Position = 1)]
		[ValidateNotNull]
		public IEnumerable<object> Collection { get; set; }

		/// <see cref="AssertionCmdletBase.Assert"/>
		protected override void Assert()
		{
			if (!Collection.Contains(Expected))
				throw new ContainsException(Expected, Message ?? "Assert-Contains Failure");
		}
	}
}