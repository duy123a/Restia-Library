using Restia.Common.Abstractions.Logger;
using Restia.Common.Extensions;
using System;
using System.IO;
using System.IO.Abstractions;
using System.Text;
using System.Threading;

namespace Restia.Common.Logger.LegacyLogger
{
	public partial class FileLogger : BaseLogger, IFileLogger
	{
		private readonly IFileSystem _fileSystem;
		private readonly object _lockObj = new object();

		public FileLogger()
		{
			_fileSystem = new FileSystem();

			lock (_lockObj)
			{
				string logDir = GlobalConfiguration.Logger.LOG_DIR_FILE_PATH;
				if (logDir.EndsWith('\\') == false)
				{
					GlobalConfiguration.Logger.LOG_DIR_FILE_PATH = logDir + @"\";
				}

				if (_fileSystem.Directory.Exists(GlobalConfiguration.Logger.LOG_DIR_FILE_PATH) == false)
				{
					_fileSystem.Directory.CreateDirectory(GlobalConfiguration.Logger.LOG_DIR_FILE_PATH);
				}
			}
		}

		public void Write(string logType, string strMessage, bool monthly = false, Encoding? encoding = null)
		{
			Write(logType, strMessage, GlobalConfiguration.Logger.LOG_DIR_FILE_PATH, monthly, encoding);
		}

		public void Write(string logType, string strMessage, string directoryPath, bool monthly = false, Encoding? encoding = null)
		{
			if ((logOutputTypeSettingList.Contains(BaseLogger.LOGTYPE_WILDCARD) == false)
				&& (logOutputTypeSettingList.Contains(logType) == false))
			{
				return;
			}

			var datePattern = monthly ? "yyyyMM" : "yyyyMMdd";
			var logFilePath = _fileSystem.Path.Combine(
				directoryPath,
				$"{logType}_{DateTime.Now.ToString(datePattern)}.{GlobalConfiguration.Logger.LOGFILE_EXTENSION}");

			encoding ??= Encoding.GetEncoding(GlobalConfiguration.Logger.LOGFILE_ENCODING);

			var mutexName = $"FileLoggerMutex_{_fileSystem.Path.GetFileName(logFilePath)}";
			using var mutex = new Mutex(false, mutexName);
			if (!mutex.WaitOne(TimeSpan.Zero, false))
			{
				return;
			}

			try
			{
				var timestamp = DateTimeOffset.Now.ToDateString();
				var message = $"[{logType}] {timestamp} {strMessage}";

				if (!_fileSystem.Directory.Exists(directoryPath))
				{
					_fileSystem.Directory.CreateDirectory(directoryPath);
				}

				using var sw = new StreamWriter(logFilePath, true, encoding);
				sw.WriteLine(message);
			}
			finally
			{
				mutex.ReleaseMutex();
			}
		}
	}
}
