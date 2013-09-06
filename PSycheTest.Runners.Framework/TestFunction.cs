using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Language;
using PSycheTest.Runners.Framework.Results;

namespace PSycheTest.Runners.Framework
{
	/// <summary>
	/// Represents a PowerShell function test.
	/// </summary>
	public class TestFunction : ITestFunction
	{
		/// <summary>
		/// Initializes a new <see cref="TestFunction"/>.
		/// </summary>
		/// <param name="functionSyntaxTree">The syntax tree of a function that represents a test</param>
		public TestFunction(FunctionDefinitionAst functionSyntaxTree)
		{
			SyntaxTree = functionSyntaxTree;
			Function = SyntaxTree.Body.GetScriptBlock();

			UniqueName = String.Format("{0}:{1}", SyntaxTree.Extent.File, SyntaxTree.Name);
			DisplayName = SyntaxTree.Name;
			FunctionName = SyntaxTree.Name;
			Source = new TestSourceInfo(
				new FileInfo(SyntaxTree.Extent.File),
				SyntaxTree.Extent.StartLineNumber,
				SyntaxTree.Extent.StartColumnNumber);

			var testAttribute = Function.Attributes.OfType<TestAttribute>().Single();
			CustomTitle = testAttribute.Title ?? string.Empty;
			ShouldSkip = !String.IsNullOrEmpty(testAttribute.SkipBecause);
			SkipReason = ShouldSkip ? testAttribute.SkipBecause : string.Empty;
		}

		/// <summary>
		/// A test's full, unique name.
		/// </summary>
		public string UniqueName { get; private set; }

		/// <summary>
		/// A test's display name.
		/// </summary>
		public string DisplayName { get; private set; }

		/// <summary>
		/// A test function's programmatic name. This may or may not be the same as <see cref="UniqueName"/>.
		/// </summary>
		public string FunctionName { get; private set; }

		/// <summary>
		/// An optional test title.
		/// </summary>
		public string CustomTitle { get; private set; }

		/// <summary>
		/// Whether a test should be skipped.
		/// </summary>
		public bool ShouldSkip { get; private set; }

		/// <summary>
		/// If <see cref="ShouldSkip"/> is true, this should be the reason
		/// for skipping a test.
		/// </summary>
		public string SkipReason { get; private set; }

		/// <summary>
		/// Contains information about where a test came from.
		/// </summary>
		public TestSourceInfo Source { get; private set; }

		/// <summary>
		/// The actual compiled function for a test.
		/// </summary>
		internal ScriptBlock Function { get; private set; }

		/// <summary>
		/// The syntax tree for the test function.
		/// </summary>
		internal FunctionDefinitionAst SyntaxTree { get; private set; }

		/// <summary>
		/// Adds a test result to a test.
		/// </summary>
		/// <param name="result">The result to add</param>
		public void AddResult(TestResult result)
		{
			_results.Add(result);
		}

		/// <see cref="ITestResultProvider.Results"/>
		public IEnumerable<TestResult> Results { get { return _results; } }

		/// <see cref="object.ToString"/>
		public override string ToString()
		{
			return UniqueName;
		}

		private readonly ICollection<TestResult> _results = new List<TestResult>(); 
	}
}