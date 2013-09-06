using System.Management.Automation;
using PSycheTest;
using PSycheTest.Exceptions;
using Xunit;

namespace Tests.Integration.PSycheTest
{
	public class AssertTrueCmdletTests : CmdletTestBase<AssertTrueCmdlet>
	{
		[Fact]
		public void Test_Assert_Failure()
		{
			// Arrange.
			AddCmdlet();
			AddParameter(p => p.Condition, false);

			// Act/Assert.
			var exception = Assert.Throws<CmdletInvocationException>(() =>
				Current.Shell.Invoke());

			Assert.NotNull(exception.InnerException);
			var assertException = Assert.IsType<ExpectedActualException>(exception.InnerException);
			Assert.Equal(true, assertException.Expected);
			Assert.Equal(false, assertException.Actual);
		}

		[Fact]
		public void Test_Assert_Success()
		{
			// Arrange.
			AddCmdlet();
			AddParameter(p => p.Condition, true);

			// Act/Assert.
			Assert.DoesNotThrow(() => Current.Shell.Invoke());
		}
	}
}