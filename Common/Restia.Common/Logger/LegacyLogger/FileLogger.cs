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
				string logDir = RestiaConfiguration.Logger.PhysicalRootPath;
				if (logDir.EndsWith('\\') == false)
				{
					RestiaConfiguration.Logger.PhysicalRootPath = logDir + @"\";
				}

				if (_fileSystem.Directory.Exists(RestiaConfiguration.Logger.PhysicalRootPath) == false)
				{
					_fileSystem.Directory.CreateDirectory(RestiaConfiguration.Logger.PhysicalRootPath);
				}
			}
		}

		public void Write(string logType, string strMessage, bool monthly = false, Encoding? encoding = null)
		{
			Write(logType, strMessage, RestiaConfiguration.Logger.PhysicalRootPath, monthly, encoding);
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
				$"{logType}_{DateTime.Now.ToString(datePattern)}.{RestiaConfiguration.Logger.LogFileExtension}");

			encoding ??= Encoding.GetEncoding(RestiaConfiguration.Logger.LogFileEncoding);

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
