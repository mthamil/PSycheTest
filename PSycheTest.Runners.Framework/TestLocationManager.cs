using System.IO;
using System.Linq;

namespace PSycheTest.Runners.Framework
{
	/// <summary>
	/// Manages a test's output and working directory, including directories.
	/// </summary>
	public class TestLocationManager : ILocationManager
	{
		/// <summary>
		/// Initializes a new <see cref="TestLocationManager"/> for a test.
		/// </summary>
		/// <param name="outputDirectory">The root working directory of a test</param>
		/// <param name="script">The current test script</param>
		/// <param name="test">The current test</param>
		public TestLocationManager(DirectoryInfo outputDirectory, ITestScript script, ITestFunction test)
		{
			_scriptDirectory = new DirectoryInfo(Path.Combine(
				outputDirectory.FullName, script.Source.Name));

			if (!_scriptDirectory.Exists)
				_scriptDirectory.Create();

			_testDirectory = _scriptDirectory.CreateSubdirectory(test.DisplayName);

			OutputDirectory = _testDirectory;
			ScriptLocation = script.Source.Directory;
		}

		/// <see cref="ILocationManager.OutputDirectory"/>
		public DirectoryInfo OutputDirectory { get; private set; }

		/// <see cref="ILocationManager.ScriptLocation"/>
		public DirectoryInfo ScriptLocation { get; private set; }

		/// <summary>
		/// Cleans up a test's output.
		/// </summary>
		public void Dispose()
		{
			if (_testDirectory.Exists && !_testDirectory.EnumerateFileSystemInfos().Any())
				_testDirectory.Delete();

			if (_scriptDirectory.Exists && !_scriptDirectory.EnumerateFileSystemInfos().Any())
				_scriptDirectory.Delete();
		}

		private readonly DirectoryInfo _testDirectory;
		private readonly DirectoryInfo _scriptDirectory;
	}
}