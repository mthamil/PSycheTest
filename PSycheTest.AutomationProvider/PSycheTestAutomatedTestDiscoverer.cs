using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using PSycheTest.Runners.Framework;
using TestCaseAutomator.AutomationProviders.Interfaces;

namespace PSycheTest.AutomationProvider
{
	/// <summary>
	/// Wraps the core PSycheTest discoverer and creates <see cref="ITestAutomation"/> objects from the
	/// discovered tests.
	/// </summary>
	[Export(typeof(ITestAutomationDiscoverer))]
	public class PSycheTestAutomatedTestDiscoverer : ITestAutomationDiscoverer
	{
		/// <summary>
		/// Initializes a new <see cref="PSycheTestAutomatedTestDiscoverer"/>. This constructor is 
		/// used by MEF.
		/// </summary>
		[ImportingConstructor]
		public PSycheTestAutomatedTestDiscoverer()
			: this(() => new PowerShellTestDiscoverer(new NullLogger()))
		{
		}

		/// <summary>
		/// Initializes a new <see cref="PSycheTestAutomatedTestDiscoverer"/> with injectable
		/// arguments.
		/// </summary>
		/// <param name="testDiscovererFactory">Creates test discoverers</param>
		public PSycheTestAutomatedTestDiscoverer(Func<IPowerShellTestDiscoverer> testDiscovererFactory)
		{
			_testDiscovererFactory = testDiscovererFactory;
		}

		/// <see cref="ITestAutomationDiscoverer.SupportedFileExtensions"/>
		public IEnumerable<string> SupportedFileExtensions { get { return _extensions; } }

		/// <see cref="ITestAutomationDiscoverer.DiscoverAutomatedTests"/>
		public IEnumerable<ITestAutomation> DiscoverAutomatedTests(IEnumerable<string> sources)
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