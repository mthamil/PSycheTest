using System;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using PSycheTest.Runners.Framework;

namespace PSycheTest.Runners.VisualStudio.Core
{
	/// <summary>
	/// An <see cref="ILogger"/> based on <see cref="IMessageLogger"/>.
	/// </summary>
	internal class VSLogger : ILogger
	{
		/// <summary>
		/// Initializes a new <see cref="VSLogger"/>.
		/// </summary>
		/// <param name="innerLogger">A Visual Studio message logger.</param>
		public VSLogger(IMessageLogger innerLogger)
		{
			_innerLogger = innerLogger;
		}

		/// <see cref="ILogger.Info"/>
		public void Info(string message, params object[] arguments)
		{
			Log(TestMessageLevel.Informational, message, arguments);
		}

		/// <see cref="ILogger.Warn"/>
		public void Warn(string message, params object[] arguments)
		{
			Log(TestMessageLevel.Warning, message, arguments);
		}

		/// <see cref="ILogger.Error"/>
		public void Error(string message, params object[] arguments)
		{
			Log(TestMessageLevel.Error, message, arguments);
		}

		private void Log(TestMessageLevel level, string message, params object[] arguments)
		{
			_innerLogger.SendMessage(level, String.Format(message, arguments));
		}

		private readonly IMessageLogger _innerLogger;
	}
}