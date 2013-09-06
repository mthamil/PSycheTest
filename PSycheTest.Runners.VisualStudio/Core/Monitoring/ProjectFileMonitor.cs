using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using PSycheTest.Runners.VisualStudio.Core.Utilities;

namespace PSycheTest.Runners.VisualStudio.Core.Monitoring
{
	/// <summary>
	/// Monitors projects for changes to child items.
	/// </summary>
	[Export(typeof(IProjectFileMonitor))]
	public class ProjectFileMonitor : IProjectFileMonitor, IVsTrackProjectDocumentsEvents2
	{
		/// <summary>
		/// Initializes a new <see cref="ProjectFileMonitor"/>.
		/// </summary>
		/// <param name="serviceProvider">Provides access to services</param>
		[ImportingConstructor]
		public ProjectFileMonitor([Import(typeof(SVsServiceProvider))] IServiceProvider serviceProvider)
		{
			ValidateArg.NotNull(serviceProvider, "serviceProvider");
			_projectDocumentTracker = serviceProvider.GetService<IVsTrackProjectDocuments2, SVsTrackProjectDocuments>();
        }

		/// <see cref="IProjectFileMonitor.FileChanged"/>
		public event EventHandler<ProjectFileChangedEventArgs> FileChanged;

		private void OnFileChanged(IVsProject project, string filePath, FileChangeType changeType)
		{
			var localEvent = FileChanged;
			if (localEvent != null)
				localEvent(this, new ProjectFileChangedEventArgs(project, filePath, changeType));
		}

		private void OnFilesChanged(int changedProjectCount, IVsProject[] projects, int[] projectStartIndices, int changedFileCount, string[] changedFiles, FileChangeType changeType)
		{
			for (int projectIndex = 0; projectIndex < changedProjectCount; projectIndex++)
			{
				var project = projects[projectIndex];
				var fileIndexUpperBound = projectIndex == changedProjectCount - 1 ? changedFileCount : projectStartIndices[projectIndex + 1];
				for (int fileIndex = projectStartIndices[projectIndex]; fileIndex < fileIndexUpperBound; fileIndex++)
				{
					var file = changedFiles[fileIndex];
					OnFileChanged(project, file, changeType);
				}
			}
		}

		/// <see cref="IProjectFileMonitor.StartMonitoring"/>
		public void StartMonitoring()
		{
			if (_projectDocumentTracker == null)
				return;

			// Subscribe to project events.
			int result = _projectDocumentTracker.AdviseTrackProjectDocumentsEvents(this, out _cookie);
			ErrorHandler.ThrowOnFailure(result);
		}

		/// <see cref="IProjectFileMonitor.StopMonitoring"/>
		public void StopMonitoring()
		{
			if (_projectDocumentTracker == null || _cookie == VSConstants.VSCOOKIE_NIL)
				return;

			// Unsubscribe from project events.
			int result = _projectDocumentTracker.UnadviseTrackProjectDocumentsEvents(_cookie);
			ErrorHandler.Succeeded(result);	// Ignore failure.

			_cookie = VSConstants.VSCOOKIE_NIL;	// Clear cookie value.
		}

		/// <see cref="IVsTrackProjectDocumentsEvents2.OnAfterAddFilesEx"/>
		public int OnAfterAddFilesEx(int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, VSADDFILEFLAGS[] rgFlags)
		{
			OnFilesChanged(cProjects, rgpProjects, rgFirstIndices, cFiles, rgpszMkDocuments, FileChangeType.Added);
			return VSConstants.S_OK;
		}

		/// <see cref="IVsTrackProjectDocumentsEvents2.OnAfterRemoveFiles"/>
		public int OnAfterRemoveFiles(int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, VSREMOVEFILEFLAGS[] rgFlags)
		{
			OnFilesChanged(cProjects, rgpProjects, rgFirstIndices, cFiles, rgpszMkDocuments, FileChangeType.Removed);
			return VSConstants.S_OK;
		}

		/// <see cref="IVsTrackProjectDocumentsEvents2.OnAfterRenameFiles"/>
		public int OnAfterRenameFiles(int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgszMkOldNames, string[] rgszMkNewNames, VSRENAMEFILEFLAGS[] rgFlags)
		{
			OnFilesChanged(cProjects, rgpProjects, rgFirstIndices, cFiles, rgszMkOldNames, FileChangeType.Removed);
			OnFilesChanged(cProjects, rgpProjects, rgFirstIndices, cFiles, rgszMkNewNames, FileChangeType.Added);
			return VSConstants.S_OK;
		}

		#region Unused Project File Events

		/// <see cref="IVsTrackProjectDocumentsEvents2.OnQueryAddFiles"/>
		public int OnQueryAddFiles(IVsProject pProject, int cFiles, string[] rgpszMkDocuments, VSQUERYADDFILEFLAGS[] rgFlags, VSQUERYADDFILERESULTS[] pSummaryResult,
		                           VSQUERYADDFILERESULTS[] rgResults)
		{
			return VSConstants.S_OK;
		}

		/// <see cref="IVsTrackProjectDocumentsEvents2.OnAfterAddDirectoriesEx"/>
		public int OnAfterAddDirectoriesEx(int cProjects, int cDirectories, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, VSADDDIRECTORYFLAGS[] rgFlags)
		{
			return VSConstants.S_OK;
		}

		/// <see cref="IVsTrackProjectDocumentsEvents2.OnAfterRemoveDirectories"/>
		public int OnAfterRemoveDirectories(int cProjects, int cDirectories, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, VSREMOVEDIRECTORYFLAGS[] rgFlags)
		{
			return VSConstants.S_OK;
		}

		/// <see cref="IVsTrackProjectDocumentsEvents2.OnQueryRenameFiles"/>
		public int OnQueryRenameFiles(IVsProject pProject, int cFiles, string[] rgszMkOldNames, string[] rgszMkNewNames, VSQUERYRENAMEFILEFLAGS[] rgFlags,
		                              VSQUERYRENAMEFILERESULTS[] pSummaryResult, VSQUERYRENAMEFILERESULTS[] rgResults)
		{
			return VSConstants.S_OK;
		}

		/// <see cref="IVsTrackProjectDocumentsEvents2.OnQueryRenameDirectories"/>
		public int OnQueryRenameDirectories(IVsProject pProject, int cDirs, string[] rgszMkOldNames, string[] rgszMkNewNames, VSQUERYRENAMEDIRECTORYFLAGS[] rgFlags,
		                                    VSQUERYRENAMEDIRECTORYRESULTS[] pSummaryResult, VSQUERYRENAMEDIRECTORYRESULTS[] rgResults)
		{
			return VSConstants.S_OK;
		}

		/// <see cref="IVsTrackProjectDocumentsEvents2.OnAfterRenameDirectories"/>
		public int OnAfterRenameDirectories(int cProjects, int cDirs, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgszMkOldNames, string[] rgszMkNewNames,
		                                    VSRENAMEDIRECTORYFLAGS[] rgFlags)
		{
			return VSConstants.S_OK;
		}

		/// <see cref="IVsTrackProjectDocumentsEvents2.OnQueryAddDirectories"/>
		public int OnQueryAddDirectories(IVsProject pProject, int cDirectories, string[] rgpszMkDocuments, VSQUERYADDDIRECTORYFLAGS[] rgFlags, VSQUERYADDDIRECTORYRESULTS[] pSummaryResult,
		                                 VSQUERYADDDIRECTORYRESULTS[] rgResults)
		{
			return VSConstants.S_OK;
		}

		/// <see cref="IVsTrackProjectDocumentsEvents2.OnQueryRemoveFiles"/>
		public int OnQueryRemoveFiles(IVsProject pProject, int cFiles, string[] rgpszMkDocuments, VSQUERYREMOVEFILEFLAGS[] rgFlags, VSQUERYREMOVEFILERESULTS[] pSummaryResult,
		                              VSQUERYREMOVEFILERESULTS[] rgResults)
		{
			return VSConstants.S_OK;
		}

		/// <see cref="IVsTrackProjectDocumentsEvents2.OnQueryRemoveDirectories"/>
		public int OnQueryRemoveDirectories(IVsProject pProject, int cDirectories, string[] rgpszMkDocuments, VSQUERYREMOVEDIRECTORYFLAGS[] rgFlags, VSQUERYREMOVEDIRECTORYRESULTS[] pSummaryResult,
		                                    VSQUERYREMOVEDIRECTORYRESULTS[] rgResults)
		{
			return VSConstants.S_OK;
		}

		/// <see cref="IVsTrackProjectDocumentsEvents2.OnAfterSccStatusChanged"/>
		public int OnAfterSccStatusChanged(int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, uint[] rgdwSccStatus)
		{
			return VSConstants.S_OK;
		}

		#endregion Unused Project File Events

		private uint _cookie = VSConstants.VSCOOKIE_NIL;
		private readonly IVsTrackProjectDocuments2 _projectDocumentTracker;
	}
}