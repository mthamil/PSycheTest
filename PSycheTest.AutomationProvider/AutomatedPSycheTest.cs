using System;
using PSycheTest.Runners.Framework;
using TestCaseAutomator.AutomationProviders.Interfaces;

namespace PSycheTest.AutomationProvider
{
	/// <summary>
	/// Represents an automated test from PSycheTest.
	/// </summary>
	public class AutomatedPSycheTest : ITestAutomation
	{
		/// <summary>
		/// Initializes a new <see cref="AutomatedPSycheTest"/>.
		/// </summary>
		/// <param name="script">The test script containing a test</param>
		/// <param name="test">The test function</param>
		public AutomatedPSycheTest(ITestScript script, ITestFunction test)
		{
			Name = String.Format("{0}:{1}", script.Source.Name, test.DisplayName);
			TestType = "Integration Test";
			Storage = script.Source.Name;
			Identifier = _identifierFactory.CreateIdentifier(Name);
		}

		/// <see cref="ITestAutomation.Identifier"/>
		public Guid Identifier { get; private set; }

		/// <see cref="ITestAutomation.Name"/>
		public string Name { get; private set; }

		/// <see cref="ITestAutomation.TestType"/>
		public string TestType { get; private set; }

		/// <see cref="ITestAutomation.Storage"/>
		public string Storage { get; private set; }

		private static readonly ITestIdentifierFactory _identifierFactory = new HashedIdentifierFactory();
	}
}