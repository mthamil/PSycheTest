using System;
using System.IO;
using System.Management.Automation;
using PSycheTest.Core;

namespace PSycheTest
{
	/// <summary>
	/// Includes a file for archival in the current test run.
	/// </summary>
	[Cmdlet("Attach", "File")]
	public class AttachFileCmdlet : PSCmdlet
	{
		/// <summary>
		/// A file to attach.
		/// </summary>
		[Parameter(Mandatory = true, Position = 0)]
		[ValidateNotNull]
		public Uri Artifact { get; set; }

		protected override void BeginProcessing()
		{
			var context = SessionState.Variables().__testContext__ as TestExecutionContext;
			if (context != null)
			{
				// Relative files are written to the current working directory.
				if (!Artifact.IsAbsoluteUri)
					Artifact = new Uri(Path.Combine(SessionState.Path.CurrentLocation.Path, Artifact.OriginalString));

				context.AttachArtifact(Artifact);
			}
		}
	}
}