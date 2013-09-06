using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Shell.Interop;
using Moq;
using PSycheTest.Runners.VisualStudio.Core.Monitoring;
using Xunit;

namespace Tests.Unit.PSycheTest.Runners.VisualStudio.Core.Monitoring
{
	public class ProjectFileMonitorTests
	{
		public ProjectFileMonitorTests()
		{
			serviceProvider.Setup(sp => sp.GetService(typeof(SVsTrackProjectDocuments)))
			               .Returns(docTracker.Object);

			monitor = new ProjectFileMonitor(serviceProvider.Object);
		}

		[Fact]
		public void Test_StartMonitoring()
		{
			// Act.
			monitor.StartMonitoring();

			// Assert.
			uint cookie;
			docTracker.Verify(s => s.AdviseTrackProjectDocumentsEvents(monitor, out cookie));
		}

		[Fact]
		public void Test_StopMonitoring()
		{
			// Arrange.
			uint cookie = 123;
			docTracker.Setup(s => s.AdviseTrackProjectDocumentsEvents(monitor, out cookie));
			monitor.StartMonitoring();

			// Act.
			monitor.StopMonitoring();

			// Assert.
			docTracker.Verify(s => s.UnadviseTrackProjectDocumentsEvents(123));
		}

		[Fact]
		public void Test_OnAfterAddFilesEx()
		{
			// Arrange.
			var projects = Mocks.Of<IVsProject>().Take(3).ToArray();
			projectFiles[projects[0]] = new[] { "file1", "file2", "file3" };
			projectFiles[projects[1]] = new[] { "file4", "file5" };
			projectFiles[projects[2]] = new[] { "file6" };

			var args = new List<ProjectFileChangedEventArgs>();
			EventHandler<ProjectFileChangedEventArgs> handler = (o, e) => args.Add(e);
			monitor.FileChanged += handler;

			// Act.
			monitor.OnAfterAddFilesEx(projects.Length, projectFiles.Select(pf => pf.Value.Count).Sum(), projects,
			                          new[] { 0, 3, 5 }, projectFiles.SelectMany(pf => pf.Value).ToArray(), new VSADDFILEFLAGS[0]);

			// Assert.
			Assert.Equal(6, args.Count);
			Assert.True(args.All(a => a.ChangeType == FileChangeType.Added));

			var firstProjectArgs = Enumerable.Range(0, 3).Select(i => args[i]).ToList();
			Assert.True(firstProjectArgs.All(a => a.Project == projects[0]));
			Assert.Equal(new[] { "file1", "file2", "file3" }, firstProjectArgs.Select(a => a.FilePath).ToArray());

			var secondProjectArgs = Enumerable.Range(3, 2).Select(i => args[i]).ToList();
			Assert.True(secondProjectArgs.All(a => a.Project == projects[1]));
			Assert.Equal(new[] { "file4", "file5" }, secondProjectArgs.Select(a => a.FilePath).ToArray());

			var thirdProjectArgs = Enumerable.Range(5, 1).Select(i => args[i]).ToList();
			Assert.True(thirdProjectArgs.All(a => a.Project == projects[2]));
			Assert.Equal(new[] { "file6" }, thirdProjectArgs.Select(a => a.FilePath).ToArray());
		}

		[Fact]
		public void Test_OnAfterRemoveFiles()
		{
			// Arrange.
			var projects = Mocks.Of<IVsProject>().Take(3).ToArray();
			projectFiles[projects[0]] = new[] { "file1", "file2", "file3" };
			projectFiles[projects[1]] = new[] { "file4", "file5" };
			projectFiles[projects[2]] = new[] { "file6" };

			var args = new List<ProjectFileChangedEventArgs>();
			EventHandler<ProjectFileChangedEventArgs> handler = (o, e) => args.Add(e);
			monitor.FileChanged += handler;

			// Act.
			monitor.OnAfterRemoveFiles(projects.Length, projectFiles.Select(pf => pf.Value.Count).Sum(), projects,
									  new[] { 0, 3, 5 }, projectFiles.SelectMany(pf => pf.Value).ToArray(), new VSREMOVEFILEFLAGS[0]);

			// Assert.
			Assert.Equal(6, args.Count);
			Assert.True(args.All(a => a.ChangeType == FileChangeType.Removed));

			var firstProjectArgs = Enumerable.Range(0, 3).Select(i => args[i]).ToList();
			Assert.True(firstProjectArgs.All(a => a.Project == projects[0]));
			Assert.Equal(new[] { "file1", "file2", "file3" }, firstProjectArgs.Select(a => a.FilePath).ToArray());

			var secondProjectArgs = Enumerable.Range(3, 2).Select(i => args[i]).ToList();
			Assert.True(secondProjectArgs.All(a => a.Project == projects[1]));
			Assert.Equal(new[] { "file4", "file5" }, secondProjectArgs.Select(a => a.FilePath).ToArray());

			var thirdProjectArgs = Enumerable.Range(5, 1).Select(i => args[i]).ToList();
			Assert.True(thirdProjectArgs.All(a => a.Project == projects[2]));
			Assert.Equal(new[] { "file6" }, thirdProjectArgs.Select(a => a.FilePath).ToArray());
		}

		[Fact]
		public void Test_OnAfterRenameFiles()
		{
			// Arrange.
			var project = Mock.Of<IVsProject>();
			var oldFiles = new[] { "file1", "file2", "file3" };
			var newFiles = new[] { "file1a", "file2a", "file3a" };

			var args = new List<ProjectFileChangedEventArgs>();
			EventHandler<ProjectFileChangedEventArgs> handler = (o, e) => args.Add(e);
			monitor.FileChanged += handler;

			// Act.
			monitor.OnAfterRenameFiles(1, 3, new[] { project }, new[] { 0 },
									   oldFiles, newFiles, new VSRENAMEFILEFLAGS[0]);

			// Assert.
			Assert.Equal(6, args.Count);
			Assert.True(args.All(a => a.Project == project));

			var removedArgs = Enumerable.Range(0, 3).Select(i => args[i]).ToList();
			Assert.True(removedArgs.All(a => a.ChangeType == FileChangeType.Removed));
			Assert.Equal(new[] { "file1", "file2", "file3" }, removedArgs.Select(a => a.FilePath).ToArray());

			var addedArgs = Enumerable.Range(3, 3).Select(i => args[i]).ToList();
			Assert.True(addedArgs.All(a => a.ChangeType == FileChangeType.Added));
			Assert.Equal(new[] { "file1a", "file2a", "file3a" }, addedArgs.Select(a => a.FilePath).ToArray());
		}

		private readonly ProjectFileMonitor monitor;

		private readonly IDictionary<IVsProject, ICollection<string>> projectFiles = new Dictionary<IVsProject, ICollection<string>>(); 
		private readonly Mock<IVsTrackProjectDocuments2> docTracker = new Mock<IVsTrackProjectDocuments2>();
		private readonly Mock<IServiceProvider> serviceProvider = new Mock<IServiceProvider>();
	}
}