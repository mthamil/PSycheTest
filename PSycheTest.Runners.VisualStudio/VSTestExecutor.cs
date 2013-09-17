using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using PSycheTest.Runners.Framework;
using PSycheTest.Runners.VisualStudio.Core;

namespace PSycheTest.Runners.VisualStudio
{
	/// <summary>
	/// An <see cref="ITestExecutor"/> that execute PowerShell test cases.
	/// </summary>
	[ExtensionUri(ExecutorUriString)]
	[Export(typeof(ITestExecutor))]
	public class VSTestExecutor : ITestExecutor
	{
		/// <summary>
		/// Initializes a new <see cref="VSTestExecutor"/>.
		/// </summary>
		[ImportingConstructor]
		public VSTestExecutor()
			: this(logger => new PowerShellTestExecutor(logger),
			       logger => new PowerShellTestDiscoverer(logger))
		{
		}

		/// <summary>
		/// Initializes a new <see cref="VSTestExecutor"/>.
		/// </summary>
		/// <param name="executorFactory">Creates text executors</param>
		/// <param name="discovererFactory">Creates test discoverers</param>
		public VSTestExecutor(
			Func<ILogger, IPowerShellTestExecutor> executorFactory,
			Func<ILogger, IPowerShellTestDiscoverer> discovererFactory)
		{
			_executorFactory = executorFactory;
			_discovererFactory = discovererFactory;
		}

		/// <see cref="ITestExecutor.RunTests(IEnumerable{TestCase},IRunContext,IFrameworkHandle)"/>
		/// <remarks>This method is executed when test cases have already been discovered or when a selected subset of tests are run.</remarks>
		public void RunTests(IEnumerable<TestCase> tests, IRunContext runContext, IFrameworkHandle frameworkHandle)
		{
			if (runContext.KeepAlive)
				frameworkHandle.EnableShutdownAfterTestRun = true;

			Channels.UnregisterAll();

			var selectedTests = new HashSet<string>(tests.Select(test => test.FullyQualifiedName));

			var logger = new VSLogger(frameworkHandle);
			var discoverer = _discovererFactory(logger);
			var scriptFiles = tests.Select(test => test.Source).Distinct().Select(source => new FileInfo(source));
			var scripts = discoverer.Discover(scriptFiles);
			RunTestsCore(scripts, test => selectedTests.Contains(test.UniqueName), logger, runContext, frameworkHandle);
		}

		/// <see cref="ITestExecutor.RunTests(IEnumerable{string},IRunContext,IFrameworkHandle)"/>
		public void RunTests(IEnumerable<string> sources, IRunContext runContext, IFrameworkHandle frameworkHandle)
		{
			if (runContext.KeepAlive)
				frameworkHandle.EnableShutdownAfterTestRun = true;

			Channels.UnregisterAll();

			var logger = new VSLogger(frameworkHandle);
			var discoverer = _discovererFactory(logger);
			var scripts = discoverer.Discover(sources.Select(s => new FileInfo(s)));
			RunTestsCore(scripts, _ => true, logger, runContext, frameworkHandle);
		}

		private void RunTestsCore(IEnumerable<ITestScript> tests, Predicate<ITestFunction> filter, ILogger logger, IRunContext runContext, ITestExecutionRecorder recorder)
		{
			var settingsService = runContext.RunSettings.GetSettings(PSycheTestRunSettings.SettingsProviderName) as IPSycheTestSettingsService ?? new PSycheTestSettingsService();

			var testCaseMap = new Dictionary<ITestFunction, TestCase>();
			var executor = _executorFactory(logger);
			executor.OutputDirectory = new DirectoryInfo(runContext.TestRunDirectory);
			logger.Info("Test output directory: {0}", executor.OutputDirectory.FullName);

			foreach (var module in settingsService.Settings.Modules)
			{
				logger.Info("Adding module '{0}'", module);
				executor.InitialModules.Add(module);
			}
			
			executor.TestStarting += (o, e) =>
			{
				var testCase = _mapper.Map(e.Test);
				testCaseMap[e.Test] = testCase;
				recorder.RecordStart(testCase);
			};
			executor.TestEnded += (o, e) =>
			{
				var testCase = testCaseMap[e.Test];
				recorder.RecordEnd(testCase, _mapper.Map(e.Result.Status));
				recorder.RecordResult(_mapper.Map(testCase, e.Result));
			};

			using (_cancellationTokenSource = new CancellationTokenSource())
			{
				executor.ExecuteAsync(tests, filter, _cancellationTokenSource.Token).Wait();	// Awaiting seems to cause odd behavior, just block instead.
			}
			_cancellationTokenSource = null;
		}

		/// <see cref="ITestExecutor.Cancel"/>
		public void Cancel()
		{
			if (_cancellationTokenSource != null)
				_cancellationTokenSource.Cancel();
		}

		private CancellationTokenSource _cancellationTokenSource;

		private readonly TestMapper _mapper = new TestMapper();

		private readonly Func<ILogger, IPowerShellTestExecutor> _executorFactory;
		private readonly Func<ILogger, IPowerShellTestDiscoverer> _discovererFactory;

		/// <summary>
		/// The Uri used to identify the <see cref="VSTestExecutor"/>.
		/// </summary>
		public const string ExecutorUriString = "executor://PSycheTest.Runners.VisualStudio/VSTestExecutor";

		/// <summary>
		/// The Uri used to identify the <see cref="VSTestExecutor"/>.
		/// </summary>
		public static readonly Uri ExecutorUri = new Uri(ExecutorUriString);
	}
}