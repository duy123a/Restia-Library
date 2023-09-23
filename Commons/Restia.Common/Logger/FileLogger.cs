using System.IO;
using System.Text;

namespace Restia.Common.Logger
{
	/// <summary>
	/// File logger
	/// </summary>
	public partial class FileLogger : BaseLogger
	{
		/// <summary>
		/// Constructor
		/// </summary>
		static FileLogger()
		{
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
			if (Directory.Exists(Constants.PHYSICALDIRPATH_LOGFILE) == false)
			{
				Directory.CreateDirectory(Constants.PHYSICALDIRPATH_LOGFILE);
			}
		}

		/// <summary>
		/// Write the log
		/// </summary>
		/// <param name="strLogType">Log type</param>
		/// <param name="strMessage">Message</param>
		/// <param name="monthly">Is monthly write</param>
		/// <param name="encoding">Encoding</param>
		public static void Write(
			string strLogType,
			string strMessage,
			bool monthly,
			Encoding encoding = null)
		{
			Write(strLogType, strMessage, Constants.PHYSICALDIRPATH_LOGFILE, monthly, encoding);
		}
		/// <summary>
		/// Write the log
		/// </summary>
		/// <param name="strLogType">Log type</param>
		/// <param name="strMessage">Message</param>
		/// <param name="directoryPath">Directory path</param>
		/// <param name="monthly">Is monthly write</param>
		/// <param name="encoding">Encoding</param>
		public static void Write(
			string strLogType,
			string strMessage,
			string directoryPath,
			bool monthly = false,
			Encoding encoding = null)
		{
			// Check log type (need to update the log type list once before using)
			if ((LOG_OUTPUT_TYPE_SETTING_LIST.Contains(BaseLogger.LOGTYPE_WILDCARD) == false)
				&& (LOG_OUTPUT_TYPE_SETTING_LIST.Contains(strLogType) == false))
			{
				return;
			}

			// Determine log file path

		}
	}
}
