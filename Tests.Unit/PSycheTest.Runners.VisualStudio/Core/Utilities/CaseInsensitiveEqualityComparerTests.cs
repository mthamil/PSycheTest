using PSycheTest.Runners.VisualStudio.Core.Utilities;
using Xunit;
using Xunit.Extensions;

namespace Tests.Unit.PSycheTest.Runners.VisualStudio.Core.Utilities
{
	public class CaseInsensitiveEqualityComparerTests
	{
		[Theory]
		[InlineData(true,  "abcde", "abcde")]
		[InlineData(true,  "abcde", "abcDE")]
		[InlineData(true,  "ABCde", "abcde")]
		[InlineData(false, "abcde", "abc")]
		[InlineData(false, "abcde", null)]
		public void Test_Equals(bool expected, string first, string second)
		{
			// Act.
			bool actual = comparer.Equals(first, second);

			// Assert.
			Assert.Equal(expected, actual);
		}

		[Theory]
		[InlineData(@"ABc_De")]
		[InlineData(@"abC_DE")]
		public void Test_GetHashCode(string input)
		{
			// Arrange.
			var expected = comparer.GetHashCode(@"abc_de");

			// Act.
			int actual = comparer.GetHashCode(input);

			// Assert.
			Assert.Equal(expected, actual);
		}

		private readonly CaseInsensitiveEqualityComparer comparer = new CaseInsensitiveEqualityComparer();
	}
}