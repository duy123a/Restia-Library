using System.IO.Abstractions;
using System.Text;

namespace Restia.Common.Logger;

public partial class FileLogger : BaseLogger
{
	private static readonly IFileSystem _fileSystem;
	private static readonly object _lockObj = new();

	static FileLogger()
	{
		_fileSystem = new FileSystem();

		lock (_lockObj)
		{
			string logDir = GlobalConfiguration.Logger.PHYSICALDIRPATH_LOGFILE;
			if (logDir.EndsWith('\\') == false)
			{
				GlobalConfiguration.Logger.PHYSICALDIRPATH_LOGFILE = logDir + @"\";
			}

			if (_fileSystem.Directory.Exists(GlobalConfiguration.Logger.PHYSICALDIRPATH_LOGFILE) == false)
			{
				_fileSystem.Directory.CreateDirectory(GlobalConfiguration.Logger.PHYSICALDIRPATH_LOGFILE);
			}
		}
	}

	public static void Write(string logType, string strMessage, bool monthly = false, Encoding? encoding = null)
	{
		Write(logType, strMessage, GlobalConfiguration.Logger.PHYSICALDIRPATH_LOGFILE, monthly, encoding);
	}

	public static void Write(string logType, string strMessage, string directoryPath, bool monthly = false, Encoding? encoding = null)
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
			var timestamp = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
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
