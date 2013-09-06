using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using PSycheTest.Runners.Framework;
using PSycheTest.Runners.VisualStudio.Core;

namespace PSycheTest.Runners.VisualStudio
{
	/// <summary>
	/// An <see cref="ITestDiscoverer"/> that parses PowerShell scripts and turns them into test cases.
	/// </summary>
	[FileExtension(TestScript.FileExtension)]
	[DefaultExecutorUri(VSTestExecutor.ExecutorUriString)]
	[Export(typeof(ITestDiscoverer))]
	public class VSTestDiscoverer : ITestDiscoverer
	{
		/// <see cref="ITestDiscoverer.DiscoverTests"/>
		public void DiscoverTests(IEnumerable<string> sources, IDiscoveryContext discoveryContext, IMessageLogger logger, ITestCaseDiscoverySink discoverySink)
		{
			Channels.UnregisterAll();

			var discoverer = new PowerShellTestDiscoverer(new VSLogger(logger));
			var tests = discoverer.Discover(sources
				.Select(s => new FileInfo(s)))
				.SelectMany(testScript => testScript.Tests)
				.Select(test => _mapper.Map(test));

			foreach (var test in tests)
				discoverySink.SendTestCase(test);
		}

		private readonly TestMapper _mapper = new TestMapper();
	}
}