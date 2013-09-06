using System.Collections.Generic;
using System.Management.Automation;
using PSycheTest;
using PSycheTest.Exceptions;
using Xunit;

namespace Tests.Integration.PSycheTest
{
	public class AssertContainsCmdletTests : CmdletTestBase<AssertContainsCmdlet>
	{
		[Fact]
		public void Test_Assert_Failure()
		{
			// Arrange.
			AddCmdlet();
			AddParameter(p => p.Expected, 1);
			AddParameter(p => p.Collection, new List<object> { 2, 3, 4 });

			// Act/Assert.
			var exception = Assert.Throws<CmdletInvocationException>(() =>
				Current.Shell.Invoke());

			Assert.NotNull(exception.InnerException);
			var assertException = Assert.IsType<ContainsException>(exception.InnerException);
			Assert.Equal(1, assertException.Expected);
		}

		[Fact]
		public void Test_Assert_Success()
		{
			// Arrange.
			AddCmdlet();
			AddParameter(p => p.Expected, 3);
			AddParameter(p => p.Collection, new List<object> { 1, 2, 3, 4 });

			// Act/Assert.
			Assert.DoesNotThrow(() => Current.Shell.Invoke());
		}
	}
}