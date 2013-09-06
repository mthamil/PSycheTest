namespace PSycheTest.Runners.Framework
{
	/// <summary>
	/// An interface for logging messages.
	/// </summary>
	public interface ILogger
	{
		/// <summary>
		/// Logs an informational message.
		/// </summary>
		/// <param name="message">The informational message</param>
		/// <param name="arguments">Any message format arguments</param>
		void Info(string message, params object[] arguments);

		/// <summary>
		/// Logs a warning message.
		/// </summary>
		/// <param name="message">The warning message</param>
		/// <param name="arguments">Any message format arguments</param>
		void Warn(string message, params object[] arguments);

		/// <summary>
		/// Logs an error message.
		/// </summary>
		/// <param name="message">The error message</param>
		/// <param name="arguments">Any message format arguments</param>
		void Error(string message, params object[] arguments);
	}
}