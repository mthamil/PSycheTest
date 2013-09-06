using System;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using PSycheTest.Runners.Framework;
using PSycheTest.Runners.Framework.Results;
using VSTestResult = Microsoft.VisualStudio.TestPlatform.ObjectModel.TestResult;
using PSTestResult = PSycheTest.Runners.Framework.Results.TestResult;

namespace PSycheTest.Runners.VisualStudio.Core
{
	/// <summary>
	/// Maps <see cref="TestFunction"/>s to <see cref="TestCase"/>s.
	/// </summary>
	internal class TestMapper
	{
		/// <summary>
		/// Maps a <see cref="TestFunction"/> to a <see cref="TestCase"/>. 
		/// </summary>
		/// <param name="test">The test to map</param>
		/// <returns>A Visual Studio <see cref="TestCase"/></returns>
		public TestCase Map(ITestFunction test)
		{
			return new TestCase(test.UniqueName, VSTestExecutor.ExecutorUri, test.Source.File.FullName)
			{
				CodeFilePath = test.Source.File.FullName,
				DisplayName = test.DisplayName,
				LineNumber = test.Source.LineNumber
			};
		}

		/// <summary>
		/// Maps a <see cref="TestStatus"/> to a <see cref="TestOutcome"/>.
		/// </summary>
		/// <param name="status">The status to map</param>
		/// <returns>A Visual Studio <see cref="TestOutcome"/></returns>
		public TestOutcome Map(TestStatus status)
		{
			switch (status)
			{
				case TestStatus.NotExecuted:
					return TestOutcome.None;
				case TestStatus.Passed:
					return TestOutcome.Passed;
				case TestStatus.Failed:
					return TestOutcome.Failed;
				case TestStatus.Skipped:
					return TestOutcome.Skipped;
			}

			return TestOutcome.None;
		}

		/// <summary>
		/// Maps a <see cref="Framework.Results.TestResult"/> to a <see cref="Microsoft.VisualStudio.TestPlatform.ObjectModel.TestResult"/>.
		/// </summary>
		/// <param name="testCase">An existing <see cref="TestCase"/></param>
		/// <param name="result">The result to map</param>
		/// <returns>A Visual Studio <see cref="Microsoft.VisualStudio.TestPlatform.ObjectModel.TestResult"/></returns>
		public VSTestResult Map(TestCase testCase, PSTestResult result)
		{
			var vsResult = new VSTestResult(testCase)
			{
				Outcome = Map(result.Status),
				Duration = result.Duration.HasValue ? result.Duration.Value : TimeSpan.Zero,
			};

			foreach (var artifact in result.Artifacts)
			{
				var attachmentSet = new AttachmentSet(artifact, "Attachment");
				attachmentSet.Attachments.Add(new UriDataAttachment(artifact, string.Empty));
				vsResult.Attachments.Add(attachmentSet);
			}

			var failedResult = result as FailedResult;
			if (failedResult != null)
			{
				vsResult.ErrorMessage = failedResult.Reason.Message;
				vsResult.ErrorStackTrace = failedResult.Reason.StackTrace;
			}

			return vsResult;
		}
	}
}