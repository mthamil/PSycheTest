using System;
using System.Collections.Generic;
using System.IO;

namespace PSycheTest.Core
{
	/// <summary>
	/// Manages test-specific state.
	/// </summary>
	public class TestExecutionContext
	{
		/// <summary>
		/// The location where test output should be stored.
		/// </summary>
		public DirectoryInfo OutputDirectory { get; set; }

		/// <summary>
		/// Any artifact files produced during a test.
		/// </summary>
		public IEnumerable<Uri> Artifacts { get { return _artifacts; } }

		/// <summary>
		/// Adds an artifact and copies it to its archival destination if it is a local file.
		/// </summary>
		/// <param name="artifact">The file to attach</param>
		public void AttachArtifact(Uri artifact)
		{
			if (artifact.IsFile)
			{
				string source = artifact.OriginalString;
				string destination = Path.Combine(OutputDirectory.FullName, Path.GetFileName(source));
				File.Copy(source, destination, true);
				_artifacts.Add(new Uri(destination));
			}
			else
			{
				_artifacts.Add(artifact);
			}
		}

		private readonly IList<Uri> _artifacts = new List<Uri>(); 
	}
}