using System;
using System.Collections.Generic;
using System.Text;

namespace Restia.Common.Logger
{
	/// <summary>
	/// Base logger
	/// </summary>
	public abstract class BaseLogger
	{
		/// <summary>Log type constant: Wild card</summary>
		public const string LOGTYPE_WILDCARD = "*";
		/// <summary>Log type constant: Debug</summary>
		public const string LOGTYPE_DEBUG = "debug";
		/// <summary>Log type constant: Info</summary>
		public const string LOGTYPE_INFO = "info";
		/// <summary>Log type constant: Warning</summary>
		public const string LOGTYPE_WARN = "warn";
		/// <summary>Log type constant: Error</summary>
		public const string LOGTYPE_ERROR = "error";
		/// <summary>Log type constant: Fatal error</summary>
		public const string LOGTYPE_FATAL = "fatal";

		/// <summary>Log output type setting list</summary>
		protected static List<string> LOG_OUTPUT_TYPE_SETTING_LIST = new List<string>();

		/// <summary>
		/// Constructor
		/// </summary>
		static BaseLogger()
		{
			// Initialization
			LOG_OUTPUT_TYPE_SETTING_LIST.Add(LOGTYPE_WILDCARD);
		}

		/// <summary>
		/// Update log output type setting list
		/// </summary>
		/// <param name="strLogOutputTypeList">Log output type list</param>
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

		/// <summary>
		/// Add logout type
		/// </summary>
		/// <param name="strLogOutputType">Log output type</param>
		private static void AddLogOutputType(string strLogOutputType)
		{
			if ((LOG_OUTPUT_TYPE_SETTING_LIST.Contains(strLogOutputType) == false)
				&& (string.IsNullOrEmpty(strLogOutputType) == false)
				&& (LOG_OUTPUT_TYPE_SETTING_LIST.Contains(LOGTYPE_WILDCARD) == false))
			{
				LOG_OUTPUT_TYPE_SETTING_LIST.Add(strLogOutputType);
			}
		}

		/// <summary>
		/// Create exception message
		/// </summary>
		/// <param name="strExceptionMessage">Custom exception message</param>
		/// <param name="ex">Exception</param>
		/// <returns>Exception message</returns>
		public static string CreateExceptionMessage(string strExceptionMessage, Exception ex)
		{
			return string.Format(
				"{0}{1}{2}",
				strExceptionMessage,
				Environment.NewLine,
				CreateExceptionMessage(ex));
		}
		/// <summary>
		/// Create exception message
		/// </summary>
		/// <param name="ex">Exception</param>
		/// <returns>Exception message</returns>
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
