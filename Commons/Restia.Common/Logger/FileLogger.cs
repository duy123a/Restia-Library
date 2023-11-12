using System;
using System.IO;
using System.IO.Abstractions;
using System.Text;
using System.Threading;

namespace Restia.Common.Logger
{
	public partial class FileLogger : BaseLogger
	{
		private static readonly IFileSystem _fileSystem;

		static FileLogger()
		{
			_fileSystem = new FileSystem();

			// Log directory name correction
			if (Constants.PHYSICALDIRPATH_LOGFILE.EndsWith(@"\") == false)
			{
				// Lock the constant so no threads can access it until it is done
				lock (Constants.PHYSICALDIRPATH_LOGFILE)
				{
					// Check again
					if (Constants.PHYSICALDIRPATH_LOGFILE.EndsWith(@"\") == false)
					{
						Constants.PHYSICALDIRPATH_LOGFILE += @"\";
					}
				}
			}

			// Check the directory exists or not
			if (_fileSystem.Directory.Exists(Constants.PHYSICALDIRPATH_LOGFILE) == false)
			{
				_fileSystem.Directory.CreateDirectory(Constants.PHYSICALDIRPATH_LOGFILE);
			}
		}

		public static void Write(
			string strLogType,
			string strMessage,
			bool monthly,
			Encoding encoding = null)
		{
			Write(strLogType, strMessage, Constants.PHYSICALDIRPATH_LOGFILE, monthly, encoding);
		}

		public static void Write(
			string strLogType,
			string strMessage,
			string directoryPath,
			bool monthly = false,
			Encoding encoding = null)
		{
			// Check log type (* mean all OK)
			if ((LOG_OUTPUT_TYPE_SETTING_LIST.Contains(BaseLogger.LOGTYPE_WILDCARD) == false)
				&& (LOG_OUTPUT_TYPE_SETTING_LIST.Contains(strLogType) == false))
			{
				return;
			}

			// Determine log file path
			var logFilePath = string.Format(
				"{0}{1}_{2}.{3}",
				directoryPath,
				strLogType,
				DateTime.Now.ToString(monthly ? "yyyyMM" : "yyyyMMdd"),
				Constants.LOGFILE_EXTENSION);

			encoding = encoding ?? Encoding.GetEncoding(Constants.LOGFILE_ENCODING);

			// Write log
			try
			{
				// Mutex controls
				using (var mutex = new Mutex(false, logFilePath.Replace("\\", "_") + ".FileWrite"))
				{
					if (mutex.WaitOne(TimeSpan.Zero, false) == false)
					{
						return;
					}

					try
					{
						using (var sw = new StreamWriter(logFilePath, true, encoding))
						{
							var message = string.Format(
								"[{0}] {1} {2}",
								strLogType,
								DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
								strMessage);
							sw.WriteLine(message);
						}
					}
					catch (Exception)
					{
						throw;
					}
					finally
					{
						mutex.ReleaseMutex();
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}
	}
}
