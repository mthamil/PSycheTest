using System.Management.Automation;
using PSycheTest;
using PSycheTest.Exceptions;
using Xunit;

namespace Tests.Integration.PSycheTest
{
	public class AssertEqualCmdletTests : CmdletTestBase<AssertEqualCmdlet>
	{
		[Fact]
		public void Test_Assert_Failure_With_Same_Type()
		{
			// Arrange.
			AddCmdlet();
			AddParameter(p => p.Expected, 1);
			AddParameter(p => p.Actual, 2);

			// Act/Assert.
			var exception = Assert.Throws<CmdletInvocationException>(() =>
				Current.Shell.Invoke());

			Assert.NotNull(exception.InnerException);
			var assertException = Assert.IsType<ExpectedActualException>(exception.InnerException);
			Assert.Equal(1, assertException.Expected);
			Assert.Equal(2, assertException.Actual);
		}

		[Fact]
		public void Test_Assert_Failure_With_Different_Types()
		{
			// Arrange.
			AddCmdlet();
			AddParameter(p => p.Expected, 1);
			AddParameter(p => p.Actual, "hello");

			// Act/Assert.
			var exception = Assert.Throws<CmdletInvocationException>(() =>
				Current.Shell.Invoke());

			Assert.NotNull(exception.InnerException);
			var assertException = Assert.IsType<ExpectedActualException>(exception.InnerException);
			Assert.Equal(1, assertException.Expected);
			Assert.Equal("hello", assertException.Actual);
		}

		[Fact]
		public void Test_Assert_Success()
		{
			// Arrange.
			AddCmdlet();
			AddParameter(p => p.Expected, 10);
			AddParameter(p => p.Actual, 10);

			// Act/Assert.
			Assert.DoesNotThrow(() => Current.Shell.Invoke());
		}
	}
}