using System;
using PSycheTest.Runners.Framework.Results;
using Xunit;

namespace Tests.Unit.PSycheTest.Runners.Framework.Results
{
	public class FailedResultTests
	{
		[Fact]
		public void Test_FailedResult()
		{
			// Act.
			var result = new FailedResult(
				TimeSpan.FromMilliseconds(13),
				new ExceptionScriptError(new InvalidOperationException()));

			// Assert.
			Assert.Equal(TimeSpan.FromMilliseconds(13), result.Duration);
			Assert.IsType<InvalidOperationException>(result.Reason.Exception);
			Assert.Equal(TestStatus.Failed, result.Status);
		}
	}
}