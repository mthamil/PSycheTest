using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using PSycheTest.Exceptions;

namespace PSycheTest
{
	/// <summary>
	/// Cmdlet that verifies that a collection does not contain a given value.
	/// </summary>
	[Cmdlet(VerbsLifecycle.Assert, "NotContains")]
	public class AssertNotContainsCmdlet : AssertionCmdletBase
	{
		/// <summary>
		/// The value that should not be found.
		/// </summary>
		[Parameter(Mandatory = true, Position = 0)]
		public object Unexpected { get; set; }

		/// <summary>
		/// The collection to check.
		/// </summary>
		[Parameter(Mandatory = true, Position = 1)]
		[ValidateNotNull]
		public IEnumerable<object> Collection { get; set; }

		/// <see cref="AssertionCmdletBase.Assert"/>
		protected override void Assert()
		{
			if (Collection.Contains(Unexpected))
				throw new NotContainsException(Unexpected, Message ?? "Assert-NotContains Failure");
		}
	}
}