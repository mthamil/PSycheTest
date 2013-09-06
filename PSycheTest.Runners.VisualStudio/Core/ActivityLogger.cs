using System;
using Microsoft.VisualStudio.Shell.Interop;
using PSycheTest.Runners.Framework;

namespace PSycheTest.Runners.VisualStudio.Core
{
	/// <summary>
	/// An <see cref="ILogger"/> that writes to the Visual Studio Activity Log.
	/// </summary>
	public class ActivityLogger : ILogger
	{
		/// <summary>
		/// Initializes a new <see cref="ActivityLogger"/>.
		/// </summary>
		/// <param name="activityLog">The Visual Studio activity log</param>
		/// <param name="logType">The type a logger is for</param>
		public ActivityLogger(IVsActivityLog activityLog, Type logType)
		{
			_activityLog = activityLog;
			_logType = logType;
		}

		/// <see cref="ILogger.Info"/>
		public void Info(string message, params object[] arguments)
		{
			Log(__ACTIVITYLOG_ENTRYTYPE.ALE_INFORMATION, message, arguments);
		}

		/// <see cref="ILogger.Warn"/>
		public void Warn(string message, params object[] arguments)
		{
			Log(__ACTIVITYLOG_ENTRYTYPE.ALE_WARNING, message, arguments);
		}

		/// <see cref="ILogger.Error"/>
		public void Error(string message, params object[] arguments)
		{
			Log(__ACTIVITYLOG_ENTRYTYPE.ALE_ERROR, message, arguments);
		}

		private void Log(__ACTIVITYLOG_ENTRYTYPE level, string message, params object[] arguments)
		{
			_activityLog.LogEntry((uint)level, _logType.Name, String.Format(message, arguments));
		}

		private readonly IVsActivityLog _activityLog;
		private readonly Type _logType;
	}
}