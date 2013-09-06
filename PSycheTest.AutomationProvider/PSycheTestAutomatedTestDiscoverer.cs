using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using PSycheTest.Runners.Framework;
using PSycheTest.Runners.Framework.Utilities.Collections;
using TestCaseAutomator.AutomationProviders.Interfaces;

namespace PSycheTest.AutomationProvider
{
	[Export(typeof(IAutomatedTestDiscoverer))]
	public class PSycheTestAutomatedTestDiscoverer : IAutomatedTestDiscoverer
	{
		public IEnumerable<string> SupportedFileExtensions { get { return TestScript.FileExtension.ToEnumerable(); } }

		public IEnumerable<IAutomatedTest> DiscoverAutomatedTests(IEnumerable<string> sources)
		{
			var testDiscoverer = new PowerShellTestDiscoverer(new NullLogger());
			var testScripts = testDiscoverer.Discover(sources.Select(f => new FileInfo(f)));

			var tests = testScripts.SelectMany(script => script.Tests, (script, test) => new { Test = test, Script = script })
				.Select(t => new AutomatedPSycheTest(t.Script, t.Test))
				.ToList();

			return tests;
		}

		private class NullLogger : ILogger
		{
			public void Info(string message, params object[] arguments)
			{
			}

			public void Warn(string message, params object[] arguments)
			{
			}

			public void Error(string message, params object[] arguments)
			{
			}
		}
	}
}