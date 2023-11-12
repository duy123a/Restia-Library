using System;
using System.Collections.Generic;
using System.Text;

namespace Restia.Common.Logger
{
	public abstract class BaseLogger
	{
		public const string LOGTYPE_WILDCARD = "*";
		public const string LOGTYPE_DEBUG = "debug";
		public const string LOGTYPE_INFO = "info";
		public const string LOGTYPE_WARN = "warn";
		public const string LOGTYPE_ERROR = "error";
		public const string LOGTYPE_FATAL = "fatal";

		protected static List<string> LOG_OUTPUT_TYPE_SETTING_LIST = new List<string>();

		static BaseLogger()
		{
			// Initialization, at the start every log is allowed
			LOG_OUTPUT_TYPE_SETTING_LIST.Add(LOGTYPE_WILDCARD);
		}

		public static void UpdateLogOutputType(string strLogOutputTypeList)
		{
			// Clear the old list
			LOG_OUTPUT_TYPE_SETTING_LIST.Clear();

			// Add new list
			foreach (string strLogType in strLogOutputTypeList.Trim().Split(','))
			{
				AddLogOutputType(strLogType);
			}
		}

		private static void AddLogOutputType(string strLogOutputType)
		{
			// If the list already contains the wildcard, then don't need to add
			if ((LOG_OUTPUT_TYPE_SETTING_LIST.Contains(strLogOutputType) == false)
				&& (string.IsNullOrEmpty(strLogOutputType) == false)
				&& (LOG_OUTPUT_TYPE_SETTING_LIST.Contains(LOGTYPE_WILDCARD) == false))
			{
				LOG_OUTPUT_TYPE_SETTING_LIST.Add(strLogOutputType);
			}
		}

		public static string CreateExceptionMessage(string strExceptionMessage, Exception ex)
		{
			return string.Format(
				"{0}{1}{2}",
				strExceptionMessage,
				Environment.NewLine,
				CreateExceptionMessage(ex));
		}

		public static string CreateExceptionMessage(Exception ex)
		{
			StringBuilder sbErrorMessage = new StringBuilder();
			while (ex != null)
			{
				sbErrorMessage.Append(
					string.Format(
						"-> {0}{1}{2}{1}",
						ex.Message,
						Environment.NewLine,
						ex.StackTrace));

				ex = ex.InnerException;
			}
			return sbErrorMessage.ToString();
		}
	}
}
