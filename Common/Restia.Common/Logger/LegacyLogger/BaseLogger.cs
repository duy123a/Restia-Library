using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Restia.Common.Logger.LegacyLogger
{
	public abstract class BaseLogger
	{
		public const string LOGTYPE_WILDCARD = "*";
		public const string LOGTYPE_DEBUG = "debug";
		public const string LOGTYPE_INFO = "info";
		public const string LOGTYPE_WARN = "warn";
		public const string LOGTYPE_ERROR = "error";
		public const string LOGTYPE_FATAL = "fatal";

		private bool _disposed = false;

		protected List<string> logOutputTypeSettingList = new List<string> { LOGTYPE_WILDCARD };

		public void UpdateLogOutputType(string strLogOutputTypeList)
		{
			logOutputTypeSettingList.Clear();

			if (strLogOutputTypeList.Trim().Contains(LOGTYPE_WILDCARD))
			{
				logOutputTypeSettingList.Add(LOGTYPE_WILDCARD);
				return;
			}

			foreach (var strLogType in strLogOutputTypeList.Trim().Split(','))
			{
				AddLogOutputType(strLogType);
			}
		}

		private void AddLogOutputType(string strLogOutputType)
		{
			if (!string.IsNullOrWhiteSpace(strLogOutputType) && !logOutputTypeSettingList.Contains(strLogOutputType))
			{
				logOutputTypeSettingList.Add(strLogOutputType);
			}
		}

		public string CreateExceptionMessage(string strExceptionMessage, Exception ex)
		{
			return $"{strExceptionMessage}{Environment.NewLine}{CreateExceptionMessage(ex)}";
		}

		public string CreateExceptionMessage(Exception? ex)
		{
			var sbErrorMessage = new StringBuilder();
			while (ex != null)
			{
				sbErrorMessage.AppendLine($"-> {ex.Message}");
				sbErrorMessage.AppendLine(ex.StackTrace);
				ex = ex.InnerException;
			}
			return sbErrorMessage.ToString();
		}

		public string CreateReplaceMessage(string messageTemplate, params object[] propertyValues)
		{
			if (propertyValues.Length == 0)
			{
				return messageTemplate;
			}

			var regex = new Regex(@"\{([^{}]+)\}");
			var matches = regex.Matches(messageTemplate);

			if (matches.Count != propertyValues.Length)
			{
				throw new ArgumentException("The number of property values ​​does not match the number of substitutions in message template.");
			}

			for (int i = 0; i < matches.Count; i++)
			{
				var match = matches[i];
				var propertyValue = propertyValues[i].ToString();
				messageTemplate = messageTemplate.Replace(match.Value, propertyValue);
			}

			return messageTemplate;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (_disposed)
				return;

			if (disposing)
			{
				// TODO: dispose managed state (managed objects)
			}

			_disposed = true;
		}

		~BaseLogger()
		{
			Dispose(false);
		}
	}
}