using System.Management.Automation;
using PSycheTest;
using PSycheTest.Exceptions;
using Xunit;

namespace Tests.Integration.PSycheTest
{
	public class AssertNotEqualCmdletTests : CmdletTestBase<AssertNotEqualCmdlet>
	{
		[Fact]
		public void Test_Assert_Failure()
		{
			// Arrange.
			AddCmdlet();
			AddParameter(p => p.First, "test");
			AddParameter(p => p.Second, "test");

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
			AddParameter(p => p.First, 13);
			AddParameter(p => p.Second, 12);

			// Act/Assert.
			Assert.DoesNotThrow(() => Current.Shell.Invoke());
		}
	}
}