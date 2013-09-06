using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace PSycheTest.Runners.Framework
{
	/// <summary>
	/// Interface for an object that executes PowerShell tests.
	/// </summary>
	public interface IPowerShellTestExecutor
	{
		/// <summary>
		/// A collection of module names that will be added to a test run's initial session state
		/// and which will persist across test executions.
		/// </summary>
		ICollection<string> InitialModules { get; }

		/// <summary>
		/// The directory where test results and artifacts should be placed.
		/// </summary>
		DirectoryInfo OutputDirectory { get; set; }

		/// <summary>
		/// Event raised when a test script is about to start.
		/// </summary>
		event EventHandler<TestScriptStartingEventArgs> TestScriptStarting;

		/// <summary>
		/// Event raised when a test is about to start.
		/// </summary>
		event EventHandler<TestStartingEventArgs> TestStarting;

		/// <summary>
		/// Event raised when a test has ended.
		/// </summary>
		event EventHandler<TestEndedEventArgs> TestEnded;

		/// <summary>
		/// Event raised when a test script has ended.
		/// </summary>
		event EventHandler<TestScriptEndedEventArgs> TestScriptEnded;

		/// <summary>
		/// Executes a collection of tests.
		/// </summary>
		/// <param name="testScripts">The test scripts to execute</param>
		/// <param name="filter">An optional predicate that determines whether a test should be run</param>
		/// <param name="cancellationToken">An optional cancellation token</param>
		Task ExecuteAsync(IEnumerable<ITestScript> testScripts, Predicate<ITestFunction> filter = null, CancellationToken cancellationToken = default(CancellationToken));
	}
}