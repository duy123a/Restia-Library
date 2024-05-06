using System;

namespace Restia.Common.Logger
{
	public interface IFileLogger : IDisposable
	{
		/// <summary>
		/// Write debug to log
		/// </summary>
		/// <param name="message">Message template describing the event.</param>
		/// <example><code>
		/// _logger.WriteDebug("Starting up at 2023/08/29 08:00:00.");
		/// </code></example>
		void WriteDebug(string message);

		/// <summary>
		/// Write debug to log
		/// </summary>
		/// <param name="messageTemplate">Message template describing the event.</param>
		/// <param name="propertyValues">Objects positionally formatted into the message template.</param>
		/// <example><code>
		/// _logger.WriteDebug("Starting up at {StartedAt}.", DateTime.Now);
		/// </code></example>
		void WriteDebug(string messageTemplate, params object[] propertyValues);


		/// <summary>
		/// Write informaion to log
		/// </summary>
		/// <param name="message">Message template describing the event.</param>
		/// <example><code>
		/// _logger.WriteInfo("Processed 2 records in 100ms.");
		/// </code></example>
		void WriteInfo(string message);

		/// <summary>
		/// Write informaion to log
		/// </summary>
		/// <param name="messageTemplate">Message template describing the event.</param>
		/// <param name="propertyValues">Objects positionally formatted into the message template.</param>
		/// <example><code>
		/// _logger.WriteInfo("Processed {RecordCount} records in {TimeMS}.", records.Length, sw.ElapsedMilliseconds);
		/// </code></example>
		void WriteInfo(string messageTemplate, params object[] propertyValues);


		/// <summary>
		///  Write error to log
		/// </summary>
		/// <param name="message">Message template describing the event.</param>
		/// <example><code>
		/// _logger.WriteError("Failed 3 records.");
		/// </code></example>
		void WriteError(string message);

		/// <summary>
		/// Write error to log
		/// </summary>
		/// <param name="messageTemplate">Message template describing the event.</param>
		/// <param name="propertyValues">Objects positionally formatted into the message template.</param>
		/// <example><code>
		/// _logger.WriteError("Failed {ErrorCount} records.", brokenRecords.Length);
		/// </code></example>
		void WriteError(string messageTemplate, params object[] propertyValues);

		/// <summary>
		/// Write error to log
		/// </summary>
		/// <param name="exception">Exception related to the event.</param>
		/// <param name="message">Message template describing the event.</param>
		/// <example><code>
		/// _logger.WriteError(ex, "Failed 3 records.");
		/// </code></example>
		void WriteError(Exception exception, string message);

		/// <summary>
		/// Write error to log
		/// </summary>
		/// <param name="exception">Exception related to the event.</param>
		/// <param name="messageTemplate">Message template describing the event.</param>
		/// <param name="propertyValues">Objects positionally formatted into the message template.</param>
		/// <example><code>
		/// _logger.WriteError(ex, "Failed {ErrorCount} records.", brokenRecords.Length);
		/// </code></example>
		void WriteError(Exception exception, string messageTemplate, params object[] propertyValues);
	}
}
