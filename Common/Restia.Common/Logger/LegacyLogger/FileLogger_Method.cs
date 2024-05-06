using System;

namespace Restia.Common.Logger.LegacyLogger
{
	public partial class FileLogger
	{
		public void WriteDebug(string message)
		{
			Write(LOGTYPE_DEBUG, message);
		}

		public void WriteDebug(string messageTemplate, params object[] propertyValues)
		{
			var replacedMessage = CreateReplaceMessage(messageTemplate, propertyValues);
			Write(LOGTYPE_DEBUG, replacedMessage);
		}

		public void WriteInfo(string message)
		{
			Write(LOGTYPE_INFO, message);
		}

		public void WriteInfo(string messageTemplate, params object[] propertyValues)
		{
			var replacedMessage = CreateReplaceMessage(messageTemplate, propertyValues);
			Write(LOGTYPE_INFO, replacedMessage);
		}

		public void WriteError(string message)
		{
			Write(LOGTYPE_ERROR, message);
		}

		public void WriteError(string messageTemplate, params object[] propertyValues)
		{
			var replacedMessage = CreateReplaceMessage(messageTemplate, propertyValues);
			Write(LOGTYPE_ERROR, replacedMessage);
		}

		public void WriteError(Exception exception, string message)
		{
			Write(LOGTYPE_ERROR, message, exception);
		}

		public void WriteError(Exception exception, string messageTemplate, params object[] propertyValues)
		{
			var replacedMessage = CreateReplaceMessage(messageTemplate, propertyValues);
			Write(LOGTYPE_ERROR, replacedMessage, exception);
		}

		private void Write(string logType, string message, Exception ex)
		{
			Write(logType, CreateExceptionMessage(message, ex));
		}
	}
}
