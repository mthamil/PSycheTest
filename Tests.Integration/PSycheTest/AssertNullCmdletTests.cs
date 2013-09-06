using System.Management.Automation;
using PSycheTest;
using PSycheTest.Exceptions;
using Xunit;

namespace Tests.Integration.PSycheTest
{
	public class AssertNullCmdletTests : CmdletTestBase<AssertNullCmdlet>
	{
		[Fact]
		public void Test_Assert_Failure()
		{
			// Arrange.
			var actual = new object();
			AddCmdlet();
			AddParameter(p => p.Actual, actual);

			// Act/Assert.
			var exception = Assert.Throws<CmdletInvocationException>(() =>
				Current.Shell.Invoke());

			Assert.NotNull(exception.InnerException);
			var assertException = Assert.IsType<ExpectedActualException>(exception.InnerException);
			Assert.Equal(null, assertException.Expected);
			Assert.Equal(actual, assertException.Actual);
		}

		[Fact]
		public void Test_Assert_Success()
		{
			// Arrange.
			AddCmdlet();
			AddParameter(p => p.Actual, null);

			// Act/Assert.
			Assert.DoesNotThrow(() => Current.Shell.Invoke());
		}
	}
}