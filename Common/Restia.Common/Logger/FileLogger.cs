using Serilog;
using Serilog.Events;
using System;
using System.Text;
using System.Threading;

namespace Restia.Common.Logger
{
	public class FileLogger : IFileLogger
	{
		/// <summary>Logger instance</summary>
		private static ILogger _logger = Serilog.Core.Logger.None;

		/// <summary>Log debug</summary>
		private string TYPE_LOG_DEBUG = $"debug_.{GlobalConfiguration.Logger.LOGFILE_EXTENSION}";
		/// <summary>Log information</summary>
		private string TYPE_LOG_INFO = $"info_.{GlobalConfiguration.Logger.LOGFILE_EXTENSION}";
		/// <summary>Log error</summary>
		private string TYPE_LOG_ERROR = $"error_.{GlobalConfiguration.Logger.LOGFILE_EXTENSION}";

		/// <summary>File size limit bytes: 10 MB</summary>
		private const long FILE_SIZE_LIMIT_BYTES = 10L * 1024 * 1024;
		/// <summary>Rolling interval</summary>
		private const RollingInterval ROLLING_INTERVAL = RollingInterval.Day;

		/// <summary>
		/// Constructor
		/// </summary>
		public FileLogger()
		{
			_logger = new LoggerConfiguration()
				.MinimumLevel.Debug()
				.WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Debug)
					.WriteTo.File(
						path: $@"{GlobalConfiguration.Logger.LOG_DIR_FILE_PATH}\{TYPE_LOG_DEBUG}",
						rollingInterval: ROLLING_INTERVAL,
						fileSizeLimitBytes: FILE_SIZE_LIMIT_BYTES,
						shared: true,
						encoding: Encoding.GetEncoding(GlobalConfiguration.Logger.LOGFILE_ENCODING)))
				.WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Information)
					.WriteTo.File(
						path: $@"{GlobalConfiguration.Logger.LOG_DIR_FILE_PATH}\{TYPE_LOG_INFO}",
						rollingInterval: ROLLING_INTERVAL,
						fileSizeLimitBytes: FILE_SIZE_LIMIT_BYTES,
						shared: true,
						encoding: Encoding.GetEncoding(GlobalConfiguration.Logger.LOGFILE_ENCODING)))
				.WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Error)
					.WriteTo.File(
						path: $@"{GlobalConfiguration.Logger.LOG_DIR_FILE_PATH}\{TYPE_LOG_ERROR}",
						rollingInterval: ROLLING_INTERVAL,
						fileSizeLimitBytes: FILE_SIZE_LIMIT_BYTES,
						shared: true,
						encoding: Encoding.GetEncoding(GlobalConfiguration.Logger.LOGFILE_ENCODING)))
				.WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Warning)
				.CreateLogger();
		}

		/// <summary>
		/// Write debug to log
		/// </summary>
		/// <param name="message">Message template describing the event.</param>
		/// <example><code>
		/// _logger.WriteDebug("Starting up at 2023/08/29 08:00:00.");
		/// </code></example>
		public void WriteDebug(string message) => WriteDebug(message, string.Empty);

		/// <summary>
		/// Write debug to log
		/// </summary>
		/// <param name="messageTemplate">Message template describing the event.</param>
		/// <param name="propertyValues">Objects positionally formatted into the message template.</param>
		/// <example><code>
		/// _logger.WriteDebug("Starting up at {StartedAt}.", DateTime.Now);
		/// </code></example>
		public void WriteDebug(string messageTemplate, params object[] propertyValues)
			=> _logger.Debug(messageTemplate, propertyValues);

		/// <summary>
		/// Write informaion to log
		/// </summary>
		/// <param name="message">Message template describing the event.</param>
		/// <example><code>
		/// _logger.WriteInfo("Processed 2 records in 100ms.");
		/// </code></example>
		public void WriteInfo(string message) => WriteInfo(message, string.Empty);

		/// <summary>
		/// Write informaion to log
		/// </summary>
		/// <param name="messageTemplate">Message template describing the event.</param>
		/// <param name="propertyValues">Objects positionally formatted into the message template.</param>
		/// <example><code>
		/// _logger.WriteInfo("Processed {RecordCount} records in {TimeMS}.", records.Length, sw.ElapsedMilliseconds);
		/// </code></example>
		public void WriteInfo(string messageTemplate, params object[] propertyValues)
			=> _logger.Information(messageTemplate, propertyValues);

		/// <summary>
		///  Write error to log
		/// </summary>
		/// <param name="message">Message template describing the event.</param>
		/// <example><code>
		/// _logger.WriteError("Failed 3 records.");
		/// </code></example>
		public void WriteError(string message) => WriteError(message, string.Empty);

		/// <summary>
		/// Write error to log
		/// </summary>
		/// <param name="messageTemplate">Message template describing the event.</param>
		/// <param name="propertyValues">Objects positionally formatted into the message template.</param>
		/// <example><code>
		/// _logger.WriteError("Failed {ErrorCount} records.", brokenRecords.Length);
		/// </code></example>
		public void WriteError(string messageTemplate, params object[] propertyValues)
			=> WriteError(null, messageTemplate, propertyValues);

		/// <summary>
		/// Write error to log
		/// </summary>
		/// <param name="exception">Exception related to the event.</param>
		/// <param name="message">Message template describing the event.</param>
		/// <example><code>
		/// _logger.WriteError(ex, "Failed 3 records.");
		/// </code></example>
		public void WriteError(Exception exception, string message) => WriteError(exception, message, string.Empty);

		/// <summary>
		/// Write error to log
		/// </summary>
		/// <param name="exception">Exception related to the event.</param>
		/// <param name="messageTemplate">Message template describing the event.</param>
		/// <param name="propertyValues">Objects positionally formatted into the message template.</param>
		/// <example><code>
		/// _logger.WriteError(ex, "Failed {ErrorCount} records.", brokenRecords.Length);
		/// </code></example>
		public void WriteError(Exception? exception, string messageTemplate, params object[] propertyValues)
			=> _logger.Error(exception, messageTemplate ?? exception?.Message ?? string.Empty, propertyValues);

		/// <summary>
		/// Dispose
		/// </summary>
		public void Dispose()
		{
			var logger = Interlocked.Exchange(ref _logger, Serilog.Core.Logger.None);
			(logger as IDisposable)?.Dispose();
			GC.SuppressFinalize(this);
		}
	}
}
