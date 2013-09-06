using System.Linq;
using PSycheTest.Runners.Framework.Utilities.Text;
using Xunit;

namespace Tests.Unit.PSycheTest.Runners.Framework.Utilities.Text
{
	public class StringExtensionsTests
	{
		[Fact]
		public void Test_Lines_With_Empty_String()
		{
			// Act.
			var lines = "".Lines();

			// Assert.
			Assert.Empty(lines);
		}

		[Fact]
		public void Test_Lines_With_One_Line()
		{
			// Act.
			var lines = "Testing 1, 2, 3".Lines();

			// Assert.
			var line = Assert.Single(lines);
			Assert.Equal("Testing 1, 2, 3", line);
		}

		[Fact]
		public void Test_Lines_With_Multiple_Lines()
		{
			// Act.
			var lines = @"Testing, 1, 2, 3
						  Hello, world
						  This is a test".Lines().ToList();

			// Assert.
			Assert.Equal(3, lines.Count);
			Assert.Equal("Testing, 1, 2, 3", lines[0]);
			Assert.Equal("Hello, world", lines[1]);
			Assert.Equal("This is a test", lines[2]);
		}
	}
}