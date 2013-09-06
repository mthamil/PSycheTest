using System;
using System.Security.Cryptography;
using System.Text;
using PSycheTest.Runners.Framework;
using TestCaseAutomator.AutomationProviders.Interfaces;

namespace PSycheTest.AutomationProvider
{
	/// <summary>
	/// Represents an automated test from PSycheTest.
	/// </summary>
	public class AutomatedPSycheTest : IAutomatedTest
	{
		/// <summary>
		/// Initializes a new <see cref="AutomatedPSycheTest"/>.
		/// </summary>
		/// <param name="script">The test script containing a test</param>
		/// <param name="test">The test function</param>
		public AutomatedPSycheTest(ITestScript script, ITestFunction test)
			: this(script, test, () => new SHA1CryptoServiceProvider())
		{
		}

		/// <summary>
		/// Initializes a new <see cref="AutomatedPSycheTest"/>.
		/// </summary>
		/// <param name="script">The test script containing a test</param>
		/// <param name="test">The test function</param>
		/// <param name="hashAlgorithmFactory">Used to create a test's unique identifier</param>
		public AutomatedPSycheTest(ITestScript script, ITestFunction test, Func<HashAlgorithm> hashAlgorithmFactory)
		{
			Name = script.Source.Name + ":" + test.DisplayName;
			TestType = "Integration Test";
			Storage = script.Source.Name;

			Identifier = CreateTestIdentifier(Name, hashAlgorithmFactory);
		}

		/// <see cref="IAutomatedTest.Identifier"/>
		public Guid Identifier { get; private set; }

		/// <see cref="IAutomatedTest.Name"/>
		public string Name { get; private set; }

		/// <see cref="IAutomatedTest.TestType"/>
		public string TestType { get; private set; }

		/// <see cref="IAutomatedTest.Storage"/>
		public string Storage { get; private set; }

		private static Guid CreateTestIdentifier(string testName, Func<HashAlgorithm> hashAlgorithmFactory)
		{
			using (var crypto = hashAlgorithmFactory())
			{
				var bytes = new byte[16];
				Array.Copy(crypto.ComputeHash(Encoding.Unicode.GetBytes(testName)), bytes, bytes.Length);
				return new Guid(bytes);
			}
		}
	}
}