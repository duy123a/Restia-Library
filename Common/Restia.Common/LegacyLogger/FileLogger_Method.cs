namespace Restia.Common.LegacyLogger;
public partial class FileLogger
{
	public static void WriteDebug(Exception ex)
	{
		WriteDebug(string.Empty, ex);
	}

	public static void WriteDebug(string message, Exception ex)
	{
		Write(LOGTYPE_DEBUG, message, ex);
	}

	public static void WriteDebug(string message)
	{
		Write(LOGTYPE_DEBUG, message);
	}

	public static void WriteInfo(Exception ex)
	{
		WriteInfo(string.Empty, ex);
	}

	public static void WriteInfo(string message, Exception ex)
	{
		Write(LOGTYPE_INFO, message, ex);
	}

	public static void WriteInfo(string message)
	{
		Write(LOGTYPE_INFO, message);
	}

	public static void WriteWarn(Exception ex)
	{
		WriteWarn(string.Empty, ex);
	}

	public static void WriteWarn(string message, Exception ex)
	{
		Write(LOGTYPE_WARN, message, ex);
	}

	public static void WriteWarn(string message)
	{
		Write(LOGTYPE_WARN, message);
	}

	public static void WriteError(Exception ex)
	{
		WriteError(string.Empty, ex);
	}

	public static void WriteError(string message, Exception ex)
	{
		Write(LOGTYPE_ERROR, message, ex);
	}

	public static void WriteError(string message)
	{
		Write(LOGTYPE_ERROR, message);
	}

	public static void WriteFatal(Exception ex)
	{
		WriteFatal(string.Empty, ex);
	}

	public static void WriteFatal(string message, Exception ex)
	{
		Write(LOGTYPE_FATAL, message, ex);
	}

	public static void WriteFatal(string message)
	{
		Write(LOGTYPE_FATAL, message);
	}

	public static void Write(string logType, string message, Exception ex)
	{
		Write(logType, CreateExceptionMessage(message, ex));
	}
}
