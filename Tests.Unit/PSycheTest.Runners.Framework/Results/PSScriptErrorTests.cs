using System;
using System.IO;
using System.Runtime.CompilerServices;
using Moq;
using PSycheTest.Runners.Framework.Results;
using Xunit;

namespace Tests.Unit.PSycheTest.Runners.Framework.Results
{
	public class PSScriptErrorTests
	{
		[Fact]
		public void Test_StackTrace()
		{
			// Arrange.
			var exception = Throw();
			var errorRecord = Mock.Of<IErrorRecord>(er =>
			                                        er.Exception == exception &&
			                                        er.ScriptStackTrace ==
			                                        @"at <ScriptBlock>, C:\Tests\Helpers.ps1: line 34
													  at Test-Things, <No file>: line 116".Trim());

			var thisFilePath = GetThisFile();

			var error = new PSScriptError(errorRecord, new FileInfo(@"C:\Tests\Test-Stuff.ps1"));

			// Act.
			var stackTrace = error.StackTrace;

			// Assert.
			Assert.Equal(String.Format(
@"at {0}.{1}() in {2}:line 60
at Helpers.ps1 in C:\Tests\Helpers.ps1:line 34
at Test-Stuff.ps1:Test-Things() in C:\Tests\Test-Stuff.ps1:line 116", GetType().FullName, "Throw", thisFilePath),
stackTrace);
		}

		[Fact]
		public void Test_Message()
		{
			// Arrange.
			var exception = Throw();
			var errorRecord = Mock.Of<IErrorRecord>(er =>
													er.Exception == exception &&
													er.ScriptStackTrace == string.Empty);

			var error = new PSScriptError(errorRecord, new FileInfo(@"C:\Tests\Test-Stuff.ps1"));

			// Act.
			var message = error.Message;

			// Assert.
			Assert.Equal("Error!", message);
		}

		private Exception Throw()
		{
			try
			{
				throw new InvalidOperationException("Error!");
			}
			catch (Exception e)
			{
				return e;
			}
		}

		private string GetThisFile([CallerFilePath] string thisFile = null)
		{
			return thisFile;
		}
	}
}