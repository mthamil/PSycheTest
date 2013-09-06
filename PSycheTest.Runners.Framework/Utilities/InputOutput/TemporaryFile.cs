using System;
using System.IO;

namespace PSycheTest.Runners.Framework.Utilities.InputOutput
{
	/// <summary>
	/// Class that manages a temporary file and aids in clean up
	/// after its use.
	/// </summary>
	public class TemporaryFile : IDisposable
	{
		/// <summary>
		/// Initializes a new <see cref="TemporaryFile"/>.
		/// </summary>
		public TemporaryFile()
		{
			File = new FileInfo(Path.GetTempFileName());
		}

		/// <summary>
		/// Initializes a new <see cref="TemporaryFile"/> with the given extension in the given directory.
		/// </summary>
		/// <param name="directory">The directory to create the file in</param>
		/// <param name="extension">The file extension to use</param>
		public TemporaryFile(DirectoryInfo directory = null, string extension = null)
		{
			string directoryPath = directory != null ? directory.FullName : Path.GetTempPath();

			string fileName = Path.GetRandomFileName();
			if (!String.IsNullOrEmpty(extension))
				fileName = Path.ChangeExtension(fileName, extension);

			File = new FileInfo(Path.Combine(directoryPath, fileName));
		}

		/// <summary>
		/// Creates an empty temporary file for the file path represented by this <see cref="TemporaryFile"/>.
		/// </summary>
		/// <remarks>This object is returned to enable a more fluent syntax.</remarks>
		public TemporaryFile Touch()
		{
			File.Create().Close();
			return this;
		}

		/// <summary>
		/// Populates a temporary file with the given content.
		/// </summary>
		/// <param name="content">The content to write to the temporary file</param>
		/// <remarks>This object is returned to enable a more fluent syntax.</remarks>
		public TemporaryFile WithContent(string content)
		{
			using (var writer = File.CreateText())
				writer.Write(content);

			return this;
		}

		/// <summary>
		/// The actual temporary file.
		/// </summary>
		public FileInfo File { get; private set; }

		#region IDisposable Implementation

		/// <summary>
		/// Implements the actual disposal logic. Subclasses should
		/// override this method to clean up resources.
		/// </summary>
		/// <param name="disposing">Whether the class is disposing from the Dispose() method</param>
		protected void Dispose(bool disposing)
		{
			if (!_isDisposed)
			{
				if (File.Exists)
					File.Delete();

				_isDisposed = true;
			}
		}

		/// <see cref="IDisposable.Dispose"/>
		public void Dispose()
		{
			Dispose(true);
			// This object will be cleaned up by the Dispose method.
			// Therefore, you should call GC.SupressFinalize to
			// take this object off the finalization queue
			// and prevent finalization code for this object
			// from executing a second time.
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Use C# destructor syntax for finalization code.
		/// This destructor will run only if the Dispose method
		/// does not get called.
		/// It gives your base class the opportunity to finalize.
		/// Do not provide destructors in types derived from this class.
		/// </summary>
		~TemporaryFile()
		{
			// Do not re-create Dispose clean-up code here.
			// Calling Dispose(false) is optimal in terms of
			// readability and maintainability.
			Dispose(false);
		}

		private bool _isDisposed;

		#endregion IDisposable Implementation
	}
}