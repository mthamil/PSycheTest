using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Language;
using PSycheTest.Runners.Framework.Results;
using PSycheTest.Runners.Framework.Utilities;

namespace PSycheTest.Runners.Framework
{
	/// <summary>
	/// Represents a PowerShell script containing tests.
	/// </summary>
	public class TestScript : ITestScript
	{
		/// <summary>
		/// Initializes a new <see cref="TestScript"/>.
		/// </summary>
		/// <param name="scriptSyntaxTree">The syntax tree for a script</param>
		/// <param name="tests">The tests belonging to the script</param>
		/// <param name="testSetup">An optional test setup function</param>
		/// <param name="testCleanup">An optional test cleanup function</param>
		public TestScript(ScriptBlockAst scriptSyntaxTree, IEnumerable<TestFunction> tests, 
			Option<FunctionDefinitionAst> testSetup, Option<FunctionDefinitionAst> testCleanup)
		{
			if (scriptSyntaxTree == null)
				throw new ArgumentNullException("scriptSyntaxTree");
			if (tests == null)
				throw new ArgumentNullException("tests");
			if (testSetup == null)
				throw new ArgumentNullException("testSetup");
			if (testCleanup == null)
				throw new ArgumentNullException("testCleanup");

			_scriptSyntaxTree = scriptSyntaxTree;
			Source = new FileInfo(_scriptSyntaxTree.Extent.File);
			ScriptBlock = _scriptSyntaxTree.GetScriptBlock();
			_tests = new List<TestFunction>(tests);
			TestSetup = testSetup;
			TestCleanup = testCleanup;
		}

		/// <summary>
		/// The file a script came from.
		/// </summary>
		public FileInfo Source { get; private set; }

		/// <summary>
		/// The text content of a script.
		/// </summary>
		public string Text
		{
			get { return _scriptSyntaxTree.Extent.Text; }
		}

		/// <summary>
		/// The tests contained with a script.
		/// </summary>
		public IEnumerable<ITestFunction> Tests { get { return _tests; } }

		/// <summary>
		/// An optional test setup function.
		/// </summary>
		public Option<FunctionDefinitionAst> TestSetup { get; private set; }

		/// <summary>
		/// An optional test cleanup function.
		/// </summary>
		public Option<FunctionDefinitionAst> TestCleanup { get; private set; }

		/// <see cref="ITestResultProvider.Results"/>
		public IEnumerable<TestResult> Results
		{
			get { return _tests.SelectMany(t => t.Results); }
		}

		/// <summary>
		/// Returns a script's results grouped by their parent <see cref="ITestFunction"/>s.
		/// </summary>
		public ILookup<ITestFunction, TestResult> ResultsByTest
		{
			get
			{
				var results = _tests.SelectMany(
					test => test.Results, 
					(test, result) => new { Test = (ITestFunction)test, Result = result });

				return results.ToLookup(t => t.Test, t => t.Result);
			}
		}

		/// <summary>
		/// The compiled script code.
		/// </summary>
		public ScriptBlock ScriptBlock { get; private set; }

		/// <see cref="object.ToString"/>
		public override string ToString()
		{
			return Source.Name;
		}

		private readonly ICollection<TestFunction> _tests; 
		private readonly ScriptBlockAst _scriptSyntaxTree;
		
		/// <summary>
		/// The file extension for PowerShell scripts.
		/// </summary>
		public const string FileExtension = ".ps1";
	}
}