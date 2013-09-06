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
	/// Monitors a solution for various events.
	/// </summary>
	[Export(typeof(ISolutionMonitor))]
	public class SolutionMonitor : IVsSolutionEvents, IVsSolutionLoadEvents, ISolutionMonitor
	{
		/// <summary>
		/// Initializes a new <see cref="SolutionMonitor"/>.
		/// </summary>
		/// <param name="serviceProvider">Used to retrieve the current solution</param>
		[ImportingConstructor]
		public SolutionMonitor([Import(typeof(SVsServiceProvider))]IServiceProvider serviceProvider)
		{
			ValidateArg.NotNull(serviceProvider, "serviceProvider");
			_currentSolution = serviceProvider.GetService<IVsSolution, SVsSolution>();
		}

		/// <see cref="ISolutionMonitor.SolutionChanged"/>
		public event EventHandler<SolutionChangedEventArgs> SolutionChanged;

		private void OnSolutionChanged(IVsSolution solution, SolutionChangeType changeType)
		{
			var localEvent = SolutionChanged;
			if (localEvent != null)
				localEvent(this, new SolutionChangedEventArgs(solution, changeType));
		}

		/// <see cref="ISolutionMonitor.ProjectChanged"/>
		public event EventHandler<ProjectChangedEventArgs> ProjectChanged;

		private void OnProjectChanged(IVsSolution solution, IVsProject project, ProjectChangeType changeType)
		{
			var localEvent = ProjectChanged;
			if (localEvent != null)
				localEvent(this, new ProjectChangedEventArgs(solution, project, changeType));
		}

		/// <see cref="ISolutionMonitor.StartMonitoring"/>
		public void StartMonitoring()
		{
			if (_currentSolution == null)
				return;

			// Subscribe to solution events.
			int result = _currentSolution.AdviseSolutionEvents(this, out _cookie);
			ErrorHandler.ThrowOnFailure(result);	// Throw on failure.
		}

		/// <see cref="ISolutionMonitor.StopMonitoring"/>
		public void StopMonitoring()
		{
			if (_currentSolution == null || _cookie == VSConstants.VSCOOKIE_NIL) 
				return;

			// Unsubscribe from solution events.
			int result = _currentSolution.UnadviseSolutionEvents(_cookie);
			ErrorHandler.Succeeded(result);	// Ignore failure.

			_cookie = VSConstants.VSCOOKIE_NIL;	// Clear cookie value.
		}

		/// <see cref="IVsSolutionEvents.OnAfterLoadProject"/>
		public int OnAfterLoadProject(IVsHierarchy pStubHierarchy, IVsHierarchy pRealHierarchy)
		{
			var project = pRealHierarchy as IVsProject;
			if (project != null)
				OnProjectChanged(_currentSolution, project, ProjectChangeType.Loaded);

			return VSConstants.S_OK;
		}

		/// <see cref="IVsSolutionEvents.OnAfterLoadProject"/>
		public int OnBeforeUnloadProject(IVsHierarchy pRealHierarchy, IVsHierarchy pStubHierarchy)
		{
			var project = pRealHierarchy as IVsProject;
			if (project != null)
				OnProjectChanged(_currentSolution, project, ProjectChangeType.Unloaded);

			return VSConstants.S_OK;
		}

		/// <see cref="IVsSolutionEvents.OnAfterLoadProject"/>
		public int OnAfterCloseSolution(object pUnkReserved)
		{
			OnSolutionChanged(_currentSolution, SolutionChangeType.Unloaded);
			return VSConstants.S_OK;
		}

		/// <see cref="IVsSolutionLoadEvents.OnAfterBackgroundSolutionLoadComplete"/>
		public int OnAfterBackgroundSolutionLoadComplete()
		{
			OnSolutionChanged(_currentSolution, SolutionChangeType.Loaded);
			return VSConstants.S_OK;
		}

		#region Unused Solution Events

		/// <see cref="IVsSolutionEvents.OnAfterLoadProject"/>
		public int OnAfterOpenProject(IVsHierarchy pHierarchy, int fAdded)
		{
			return VSConstants.S_OK;
		}

		/// <see cref="IVsSolutionEvents.OnAfterLoadProject"/>
		public int OnQueryCloseProject(IVsHierarchy pHierarchy, int fRemoving, ref int pfCancel)
		{
			return VSConstants.S_OK;
		}

		/// <see cref="IVsSolutionEvents.OnAfterLoadProject"/>
		public int OnBeforeCloseProject(IVsHierarchy pHierarchy, int fRemoved)
		{
			return VSConstants.S_OK;
		}

		/// <see cref="IVsSolutionEvents.OnAfterLoadProject"/>
		public int OnQueryUnloadProject(IVsHierarchy pRealHierarchy, ref int pfCancel)
		{
			return VSConstants.S_OK;
		}

		/// <see cref="IVsSolutionEvents.OnQueryCloseSolution"/>
		public int OnQueryCloseSolution(object pUnkReserved, ref int pfCancel)
		{
			return VSConstants.S_OK;
		}

		/// <see cref="IVsSolutionEvents.OnAfterLoadProject"/>
		public int OnBeforeCloseSolution(object pUnkReserved)
		{
			return VSConstants.S_OK;
		}

		/// <see cref="IVsSolutionEvents.OnAfterLoadProject"/>
		public int OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
		{
			return VSConstants.S_OK;
		}

		/// <see cref="IVsSolutionLoadEvents.OnBeforeOpenSolution"/>
		public int OnBeforeOpenSolution(string pszSolutionFilename)
		{
			return VSConstants.S_OK;
		}

		/// <see cref="IVsSolutionLoadEvents.OnBeforeBackgroundSolutionLoadBegins"/>
		public int OnBeforeBackgroundSolutionLoadBegins()
		{
			return VSConstants.S_OK;
		}

		/// <see cref="IVsSolutionLoadEvents.OnQueryBackgroundLoadProjectBatch"/>
		public int OnQueryBackgroundLoadProjectBatch(out bool pfShouldDelayLoadToNextIdle)
		{
			pfShouldDelayLoadToNextIdle = false;
			return VSConstants.S_OK;
		}

		/// <see cref="IVsSolutionLoadEvents.OnBeforeLoadProjectBatch"/>
		public int OnBeforeLoadProjectBatch(bool fIsBackgroundIdleBatch)
		{
			return VSConstants.S_OK;
		}

		/// <see cref="IVsSolutionLoadEvents.OnAfterLoadProjectBatch"/>
		public int OnAfterLoadProjectBatch(bool fIsBackgroundIdleBatch)
		{
			return VSConstants.S_OK;
		}

		#endregion Unused Solution Events

		private uint _cookie = VSConstants.VSCOOKIE_NIL;
		private readonly IVsSolution _currentSolution;
	}
}