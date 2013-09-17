using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using PSycheTest.Runners.Framework;
using TestCaseAutomator.AutomationProviders.Interfaces;

namespace PSycheTest.AutomationProvider
{
	[Export(typeof(IAutomatedTestDiscoverer))]
	public class PSycheTestAutomatedTestDiscoverer : IAutomatedTestDiscoverer
	{
		[ImportingConstructor]
		public PSycheTestAutomatedTestDiscoverer()
			: this(() => new PowerShellTestDiscoverer(new NullLogger()))
		{
		}

		public PSycheTestAutomatedTestDiscoverer(Func<IPowerShellTestDiscoverer> testDiscovererFactory)
		{
			_testDiscovererFactory = testDiscovererFactory;
		}

		public IEnumerable<string> SupportedFileExtensions { get { return _extensions; } }

		public IEnumerable<IAutomatedTest> DiscoverAutomatedTests(IEnumerable<string> sources)
		{
			var testDiscoverer = _testDiscovererFactory();
			var testScripts = testDiscoverer.Discover(
				sources.Where(s => _extensions.Contains(Path.GetExtension(s)))
				       .Select(f => new FileInfo(f)));

			var tests = testScripts.SelectMany(script => script.Tests, (script, test) => new { Test = test, Script = script })
				.Select(t => new AutomatedPSycheTest(t.Script, t.Test))
				.ToList();

			return tests;
		}

		private readonly Func<IPowerShellTestDiscoverer> _testDiscovererFactory;

		private static readonly ICollection<string> _extensions = new HashSet<string> { TestScript.FileExtension }; 

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