using System.Management.Automation;
using PSycheTest.Core;
using Tests.Support;
using Xunit;

namespace Tests.Unit.PSycheTest.Core
{
	public class SessionStateExtensionsTests
	{
		[Fact]
		[UseRunspace]
		public void Test_Set_Variable()
		{
			// Arrange.
			var sessionState = new SessionState();

			// Act.
			sessionState.Variables().aTestVariable = "testing";

			// Assert.
			Assert.Equal("testing", sessionState.PSVariable.GetValue("aTestVariable"));
		}

		[Fact]
		[UseRunspace]
		public void Test_Get_Variable()
		{
			// Arrange.
			var sessionState = new SessionState();
			sessionState.PSVariable.Set("aTestVariable", "testing");

			// Act.
			string value = sessionState.Variables().aTestVariable;

			// Assert.
			Assert.Equal("testing", value);
		}

		[Fact]
		[UseRunspace]
		public void Test_Unset_Variable()
		{
			// Arrange.
			var sessionState = new SessionState();

			// Act.
			object value = sessionState.Variables().aTestVariable;

			// Assert.
			Assert.Null(value);
		} 
	}
}