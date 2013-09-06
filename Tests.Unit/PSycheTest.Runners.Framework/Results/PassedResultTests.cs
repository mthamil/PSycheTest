using System;
using PSycheTest.Runners.Framework.Results;
using PSycheTest.Runners.Framework.Utilities.Collections;
using Xunit;

namespace Tests.Unit.PSycheTest.Runners.Framework.Results
{
	public class PassedResultTests
	{
		[Fact]
		public void Test_PassedResult()
		{
			// Act.
			var result = new PassedResult(
				TimeSpan.FromMilliseconds(5), new Uri(@"C:\output.txt").ToEnumerable());

			// Assert.
			Assert.Equal(TimeSpan.FromMilliseconds(5), result.Duration);
			Assert.Equal(TestStatus.Passed, result.Status);
			var artifact = Assert.Single(result.Artifacts);
			Assert.Equal(@"C:\output.txt", artifact.OriginalString);
		} 
	}
}