using System;
using System.IO;
using PSycheTest.Runners.Framework.Utilities.InputOutput;
using Xunit;

namespace Tests.Unit.PSycheTest.Runners.Framework.Utilities.InputOutput
{
	public class TemporaryFileTests
	{
		[Fact]
		public void Test_File()
		{
			// Arrange.
			using (var temp = new TemporaryFile())
			{
				// Act.
				var file = temp.File;

				// Assert.
				Assert.NotNull(file);
				Assert.Equal(Path.GetTempPath().TrimEnd(Path.DirectorySeparatorChar), file.DirectoryName.TrimEnd(Path.DirectorySeparatorChar));
			}
		}

		[Fact]
		public void Test_File_With_Extension()
		{
			// Arrange.
			using (var temp = new TemporaryFile(extension:".test"))
			{
				// Act.
				var file = temp.File;

				// Assert.
				Assert.NotNull(file);
				Assert.Equal(Path.GetTempPath().TrimEnd(Path.DirectorySeparatorChar), file.DirectoryName.TrimEnd(Path.DirectorySeparatorChar));
				Assert.True(file.FullName.EndsWith(".test"));
			}
		}

		[Fact]
		public void Test_File_In_Directory()
		{
			// Arrange.
			var path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
			using (var temp = new TemporaryFile(new DirectoryInfo(path), ".test"))
			{
				// Act.
				var file = temp.File;

				// Assert.
				Assert.NotNull(file);
				Assert.Equal(path.TrimEnd(Path.DirectorySeparatorChar), file.DirectoryName.TrimEnd(Path.DirectorySeparatorChar));
				Assert.True(file.FullName.EndsWith(".test"));
			}
		}

		[Fact]
		public void Test_Touch()
		{
			// Arrange.
			using (var temp = new TemporaryFile())
			{
				// Act.
				var returned = temp.Touch();

				// Assert.
				Assert.True(temp.File.Exists);
				Assert.Same(temp, returned);
			}
		}

		[Fact]
		public void Test_WithContent()
		{
			// Arrange.
			using (var temp = new TemporaryFile())
			{
				// Act.
				var returned = temp.WithContent("stuff");

				// Assert.
				Assert.True(temp.File.Exists);
				Assert.Equal("stuff", File.ReadAllText(temp.File.FullName));
				Assert.Same(temp, returned);
			}
		}

		[Fact]
		public void Test_Dispose()
		{
			// Arrange.
			var temp = new TemporaryFile().Touch();

			// Act.
			temp.Dispose();
			temp.File.Refresh();

			// Assert.
			Assert.False(temp.File.Exists);
		}

		[Fact]
		public void Test_Destructor()
		{
			// Act.
			FileInfo file = GetTempFile();

			GC.Collect();
			GC.WaitForPendingFinalizers();
			file.Refresh();

			// Assert.
			Assert.False(file.Exists);
		}

		private FileInfo GetTempFile()
		{
			var temp = new TemporaryFile().Touch();
			return temp.File;
		} 
	}
}