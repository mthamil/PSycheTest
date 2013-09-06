using System.Management.Automation.Runspaces;
using System.Reflection;
using Xunit;

namespace Tests.Support
{
	/// <summary>
	/// Populates <see cref="Runspace.DefaultRunspace"/> for a test and then restores the original.
	/// </summary>
	public class UseRunspaceAttribute : BeforeAfterTestAttribute
	{
		public override void Before(MethodInfo methodUnderTest)
		{
			_original = Runspace.DefaultRunspace;
			Runspace.DefaultRunspace = RunspaceFactory.CreateRunspace();
			Runspace.DefaultRunspace.Open();
		}

		public override void After(MethodInfo methodUnderTest)
		{
			Runspace.DefaultRunspace.Dispose();
			Runspace.DefaultRunspace = _original;
		}

		private Runspace _original;
	}
}