using System;
using System.IO;

namespace PSycheTest.Runners.Framework.Utilities.InputOutput
{
	/// <summary>
	/// Class that manages a temporary directory and aids in clean up
	/// after its use.
	/// </summary>
	public class TemporaryDirectory : IDisposable
	{
		/// <summary>
		/// Initializes a new <see cref="TemporaryDirectory"/> in the given directory.
		/// </summary>
		/// <param name="parent">The parent directory to use</param>
		/// <param name="name">The name of the directory</param>
		public TemporaryDirectory(DirectoryInfo parent = null, string name = null)
		{
			string parentPath = parent != null ? parent.FullName : Path.GetTempPath();
			name = name ?? Path.GetRandomFileName();

			Directory = new DirectoryInfo(Path.Combine(parentPath, name));
			Directory.Create();
		}

		/// <summary>
		/// The actual temporary directory.
		/// </summary>
		public DirectoryInfo Directory { get; private set; }

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
				if (Directory.Exists)
					Directory.Delete(true);

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
		~TemporaryDirectory()
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