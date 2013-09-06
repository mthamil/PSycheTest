using System;
using PSycheTest.Runners.Framework.Results;
using Xunit;

namespace Tests.Unit.PSycheTest.Runners.Framework.Results
{
	public class ExceptionScriptErrorTests
	{
		[Fact]
		public void Test_StackTrace_When_Exception_Has_Null_StackTrace()
		{
			// Arrange.
			var error = new ExceptionScriptError(new Exception());

			// Act.
			var stackTrace = error.StackTrace;

			// Assert.
			Assert.Equal(string.Empty, stackTrace);
		}

		[Fact]
		public void Test_Message()
		{
			// Arrange.
			var error = new ExceptionScriptError(new Exception("Oops!"));

			// Act.
			var message = error.Message;

			// Assert.
			Assert.Equal("Oops!", message);
		}
	}
}