using System;
using System.IO;
using PSycheTest.Runners.Framework.Utilities.InputOutput;
using Xunit;

namespace Tests.Unit.PSycheTest.Runners.Framework.Utilities.InputOutput
{
	public class TemporaryDirectoryTests
	{
		[Fact]
		public void Test_Directory()
		{
			// Arrange.
			using (var temp = new TemporaryDirectory())
			{
				// Act.
				var directory = temp.Directory;

				// Assert.
				Assert.NotNull(directory);
				Assert.True(directory.Exists);
				Assert.Equal(Path.GetTempPath().TrimEnd(Path.DirectorySeparatorChar), directory.Parent.FullName.TrimEnd(Path.DirectorySeparatorChar));
			}
		}

		[Fact]
		public void Test_Directory_With_Name()
		{
			// Arrange.
			using (var temp = new TemporaryDirectory(name: "testDir"))
			{
				// Act.
				var directory = temp.Directory;

				// Assert.
				Assert.NotNull(directory);
				Assert.True(directory.Exists);
				Assert.Equal(Path.GetTempPath().TrimEnd(Path.DirectorySeparatorChar), directory.Parent.FullName.TrimEnd(Path.DirectorySeparatorChar));
				Assert.Equal("testDir", directory.Name);
			}
		}

		[Fact]
		public void Test_Directory_In_Parent()
		{
			// Arrange.
			var parent = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
			using (var temp = new TemporaryDirectory(new DirectoryInfo(parent)))
			{
				// Act.
				var directory = temp.Directory;

				// Assert.
				Assert.NotNull(directory);
				Assert.True(directory.Exists);
				Assert.Equal(parent.TrimEnd(Path.DirectorySeparatorChar), directory.Parent.FullName.TrimEnd(Path.DirectorySeparatorChar));
			}
		}

		[Fact]
		public void Test_Dispose()
		{
			// Arrange.
			var temp = new TemporaryDirectory();

			// Act.
			temp.Dispose();
			temp.Directory.Refresh();

			// Assert.
			Assert.False(temp.Directory.Exists);
		}

		[Fact]
		public void Test_Destructor()
		{
			// Act.
			var directory = GetTempDir();

			GC.Collect();
			GC.WaitForPendingFinalizers();
			directory.Refresh();

			// Assert.
			Assert.False(directory.Exists);
		}

		private DirectoryInfo GetTempDir()
		{
			var temp = new TemporaryDirectory();
			return temp.Directory;
		} 
	}
}