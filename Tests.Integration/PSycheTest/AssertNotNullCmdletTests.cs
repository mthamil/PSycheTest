using System.Management.Automation;
using PSycheTest;
using PSycheTest.Exceptions;
using Xunit;

namespace Tests.Integration.PSycheTest
{
	public class AssertNotNullCmdletTests : CmdletTestBase<AssertNotNullCmdlet>
	{
		[Fact]
		public void Test_Assert_Failure()
		{
			// Arrange.
			AddCmdlet();
			AddParameter(p => p.Value, null);

			// Act/Assert.
			var exception = Assert.Throws<CmdletInvocationException>(() =>
				Current.Shell.Invoke());

			Assert.NotNull(exception.InnerException);
			Assert.IsType<AssertionException>(exception.InnerException);
		}

		[Fact]
		public void Test_Assert_Success()
		{
			// Arrange.
			AddCmdlet();
			AddParameter(p => p.Value, new object());

			// Act/Assert.
			Assert.DoesNotThrow(() => Current.Shell.Invoke());
		}
	}
}