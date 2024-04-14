using System.Text;

namespace Restia.Common.LegacyLogger;

public abstract class BaseLogger
{
	public const string LOGTYPE_WILDCARD = "*";
	public const string LOGTYPE_DEBUG = "debug";
	public const string LOGTYPE_INFO = "info";
	public const string LOGTYPE_WARN = "warn";
	public const string LOGTYPE_ERROR = "error";
	public const string LOGTYPE_FATAL = "fatal";

	protected static List<string> logOutputTypeSettingList = [LOGTYPE_WILDCARD];

	public static void UpdateLogOutputType(string strLogOutputTypeList)
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

	private static void AddLogOutputType(string strLogOutputType)
	{
		if (!string.IsNullOrWhiteSpace(strLogOutputType) && !logOutputTypeSettingList.Contains(strLogOutputType))
		{
			logOutputTypeSettingList.Add(strLogOutputType);
		}
	}

	public static string CreateExceptionMessage(string strExceptionMessage, Exception ex)
	{
		return $"{strExceptionMessage}{Environment.NewLine}{CreateExceptionMessage(ex)}";
	}

	public static string CreateExceptionMessage(Exception? ex)
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
}