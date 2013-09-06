using PSycheTest.Runners.Framework.Results;
using Xunit;

namespace Tests.Unit.PSycheTest.Runners.Framework.Results
{
	public class SkippedResultTests
	{
		[Fact]
		public void Test_SkippedResult()
		{
			// Act.
			var result = new SkippedResult("Skipped");

			// Assert.
			Assert.Null(result.Duration);
			Assert.Equal(TestStatus.Skipped, result.Status);
			Assert.Equal("Skipped", result.SkipReason);
		}
	}
}