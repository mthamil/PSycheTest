using System;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using Moq;
using PSycheTest.Runners.Framework;
using PSycheTest.Runners.Framework.Utilities.InputOutput;
using Tests.Support;
using Xunit;

namespace Tests.Unit.PSycheTest.Runners.Framework
{
	public class TestExecutionTransactionTests
	{
		[Fact]
		public void Test_Transaction_Disposal_Clears_Streams_And_Commands()
		{
			// Arrange.
			using (var runspace = RunspaceFactory.CreateRunspace(InitialSessionState.Create()))
			{
				runspace.Open();
				using (var powershell = PowerShell.Create())
				using (var tempDir = new TemporaryDirectory())
				{
					var transaction = new TestExecutionTransaction(powershell, 
						Mock.Of<ILocationManager>(m => 
							m.OutputDirectory == tempDir.Directory && 
							m.ScriptLocation == tempDir.Directory));

					powershell.Runspace = runspace;
					powershell.Commands.AddCommand("Get-Host");
					powershell.Commands.AddCommand("Get-Random");
					powershell.Streams.Error.Add(new ErrorRecord(new InvalidOperationException(), "10", ErrorCategory.InvalidOperation, new object()));

					// Act.
					transaction.Dispose();

					// Assert.
					Assert.Empty(powershell.Commands.Commands);
					Assert.Empty(powershell.Streams.Error);
				}
			}
		}
	}
}