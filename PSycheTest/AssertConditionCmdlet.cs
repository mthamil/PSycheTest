using System.Management.Automation;

namespace PSycheTest
{
	/// <summary>
	/// Base class for a cmdlet that verifies the truth of a statement.
	/// </summary>
	public abstract class AssertConditionCmdlet : AssertionCmdletBase
	{
		/// <summary>
		/// The expected value.
		/// </summary>
		[Parameter(Mandatory = true, Position = 0)]
		public bool Condition { get; set; }
	}
}