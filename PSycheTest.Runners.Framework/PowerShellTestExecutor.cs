using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using PSycheTest.Core;
using PSycheTest.Runners.Framework.Extensions;
using PSycheTest.Runners.Framework.Results;
using PSycheTest.Runners.Framework.Timers;
using PSycheTest.Runners.Framework.Utilities.Collections;

namespace PSycheTest.Runners.Framework
{
	/// <summary>
	/// Executes PowerShell tests.
	/// </summary>
	public class PowerShellTestExecutor : IPowerShellTestExecutor
	{
		/// <summary>
		/// Initializes a new <see cref="PowerShellTestExecutor"/>.
		/// </summary>
		/// <param name="logger">A message logger</param>
		public PowerShellTestExecutor(ILogger logger)
			: this(logger, () => new StopwatchTimer(), TaskScheduler.Default)
		{
		}

		/// <summary>
		/// Initializes a new <see cref="PowerShellTestExecutor"/>.
		/// </summary>
		/// <param name="logger">A message logger</param>
		/// <param name="timerFactory">Creates new <see cref="ITestTimer"/>s</param>
		/// <param name="taskScheduler">The scheduler to use for tasks</param>
		public PowerShellTestExecutor(ILogger logger, Func<ITestTimer> timerFactory, TaskScheduler taskScheduler)
		{
			_logger = logger;
			_timerFactory = timerFactory;
			_taskScheduler = taskScheduler;
			_testTransactionFactory = (p, ts, tf, d) => new TestExecutionTransaction(p, new TestLocationManager(d, ts, tf));

			InitialModules = new List<string>
			{
				Assembly.GetAssembly(typeof(AssertionCmdletBase)).Location
			};

			_initialSessionState = new Lazy<InitialSessionState>(() =>
			{
				var initialState = InitialSessionState.CreateDefault();
				initialState.ThrowOnRunspaceOpenError = true;
				foreach (var moduleName in InitialModules)
					initialState.ImportPSModule(new[] { moduleName });

				return initialState;
			});
		}

		/// <summary>
		/// A collection of modules that will be added to a test run's initial session state
		/// and which will persist across test executions.
		/// </summary>
		public ICollection<string> InitialModules { get; private set; }

		/// <summary>
		/// The root directory where test results and artifacts should be placed.
		/// </summary>
		public DirectoryInfo OutputDirectory { get; set; }

		#region Events

		/// <summary>
		/// Event raised when a test script is about to start.
		/// </summary>
		public event EventHandler<TestScriptStartingEventArgs> TestScriptStarting;

		private void OnTestScriptStarting(ITestScript script)
		{
			var localEvent = TestScriptStarting;
			if (localEvent != null)
				localEvent(this, new TestScriptStartingEventArgs(script));
		}


		/// <summary>
		/// Event raised when a test is about to start.
		/// </summary>
		public event EventHandler<TestStartingEventArgs> TestStarting;

		private void OnTestStarting(ITestFunction test)
		{
			var localEvent = TestStarting;
			if (localEvent != null)
				localEvent(this, new TestStartingEventArgs(test));
		}

		/// <summary>
		/// Event raised when a test has ended.
		/// </summary>
		public event EventHandler<TestEndedEventArgs> TestEnded;

		private void OnTestEnded(ITestFunction test, TestResult result)
		{
			var localEvent = TestEnded;
			if (localEvent != null)
				localEvent(this, new TestEndedEventArgs(test, result));
		}

		/// <summary>
		/// Event raised when a test script has ended.
		/// </summary>
		public event EventHandler<TestScriptEndedEventArgs> TestScriptEnded;

		private void OnTestScriptEnded(ITestScript script)
		{
			var localEvent = TestScriptEnded;
			if (localEvent != null)
				localEvent(this, new TestScriptEndedEventArgs(script));
		}

		#endregion Events

		/// <summary>
		/// Executes a collection of tests.
		/// </summary>
		/// <param name="testScripts">The test scripts to execute</param>
		/// <param name="filter">An optional predicate that determines whether a test should be run</param>
		/// <param name="cancellationToken">An optional cancellation token</param>
		public async Task ExecuteAsync(IEnumerable<ITestScript> testScripts, Predicate<ITestFunction> filter = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			filter = filter ?? (_ => true);

			using (var runspace = RunspaceFactory.CreateRunspace(_initialSessionState.Value))
			{
				runspace.Open();

				foreach (var testScript in testScripts)
				{
					_logger.Info("Executing test script: '{0}':", testScript.Source.FullName);

					OnTestScriptStarting(testScript);
					using (var powershell = PowerShell.Create(RunspaceMode.NewRunspace))
					using (var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
					{
						linkedTokenSource.Token.Register(() =>
						{
							_logger.Info("Cancelling test execution...");
							powershell.Stop();
						});

						powershell.Runspace = runspace;

						foreach (var test in testScript.Tests.Where(test => filter(test)))
						{
							_logger.Info("Executing test '{0}'", test.DisplayName);

							cancellationToken.ThrowIfCancellationRequested();

							using (var transaction = _testTransactionFactory(powershell, testScript, test, OutputDirectory))
							{
								OnTestStarting(test);
								var results = await ExecuteAsync(transaction, powershell, test, testScript).ConfigureAwait(false);
								foreach (var result in results)
								{
									_logger.Info("Test '{0}': {1}", test.DisplayName, result.Status);
									test.AddResult(result);
									OnTestEnded(test, result);
								}
							}
						}
					}
					OnTestScriptEnded(testScript);
				}
			}
		}

		/// <summary>
		/// Executes a test function.
		/// </summary>
		/// <param name="transaction">The current test transaction</param>
		/// <param name="powershell">The PowerShell instance to us</param>
		/// <param name="test">The test to run</param>
		/// <param name="script">The parent script</param>
		private async Task<IEnumerable<TestResult>> ExecuteAsync(ITestExecutionTransaction transaction, PowerShell powershell, ITestFunction test, ITestScript script)
		{
			var timer = _timerFactory();
			try
			{
				if (test.ShouldSkip)
					return new SkippedResult(test.SkipReason).ToEnumerable();

				var testContext = InitializeTestContext(powershell, transaction.OutputDirectory);
				IReadOnlyCollection<ErrorRecord> errors;
				using (timer.Start())
				{
					errors = await ExecuteCoreAsync(powershell, test, script).ConfigureAwait(false);
				}

				if (errors.Any())
					return new FailedResult(timer.Elapsed, new PSScriptError(new ErrorRecordWrapper(errors.First()), test.Source.File)).ToEnumerable();

				return new PassedResult(timer.Elapsed, testContext.Artifacts).ToEnumerable();
			}
			catch (CmdletInvocationException cmdletException)
			{
				return CreateFailed(timer.Elapsed, cmdletException.InnerException ?? cmdletException).ToEnumerable();
			}
			catch (Exception exception)
			{
				_logger.Error("Exception occurred during test '{0}': {1}{2}{3}", test.UniqueName, exception.Message, Environment.NewLine, exception.StackTrace);
				return CreateFailed(timer.Elapsed, exception).ToEnumerable();
			}
		}

		private async Task<IReadOnlyCollection<ErrorRecord>> ExecuteCoreAsync(PowerShell powershell, ITestFunction test, ITestScript script)
		{
			// Add the script's functions/variables to the pipeline.
			powershell.AddScript(script.Text);
			var scriptErrors = await powershell.InvokeAsync(_taskScheduler).ConfigureAwait(false);
			powershell.Commands.Clear();	// Clear the pipeline.
			if (scriptErrors.Any())
				return scriptErrors;

			// Now execute the test function plus setup/teardown.
			script.TestSetup.Apply(s => powershell.AddCommand(s.Name));
			powershell.AddCommand(test.FunctionName);
			script.TestCleanup.Apply(s => powershell.AddCommand(s.Name));

			var testErrors = await powershell.InvokeAsync(_taskScheduler).ConfigureAwait(false);
			if (testErrors.Any())
				return testErrors;

			return new ErrorRecord[0];
		}

		private static TestExecutionContext InitializeTestContext(PowerShell powershell, DirectoryInfo outputDirectory)
		{
			var testContext = new TestExecutionContext
			{
				OutputDirectory = outputDirectory
			};
			
			powershell.Runspace.SessionStateProxy.Variables().__testContext__ = testContext;
			return testContext;
		}

		private static TestResult CreateFailed(TimeSpan elapsed, Exception exception)
		{
			return new FailedResult(elapsed, new ExceptionScriptError(exception));
		}

		private readonly Lazy<InitialSessionState> _initialSessionState;

		private readonly ILogger _logger;
		private readonly Func<ITestTimer> _timerFactory;
		private readonly Func<PowerShell, ITestScript, ITestFunction, DirectoryInfo, ITestExecutionTransaction> _testTransactionFactory; 
		private readonly TaskScheduler _taskScheduler;
	}
}