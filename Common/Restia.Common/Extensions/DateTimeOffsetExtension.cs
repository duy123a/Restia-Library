using System;

namespace Restia.Common.Extensions
{
	public static class DateTimeOffsetExtension
	{
		public const string FILECONTENT_LASTEXEC_DATEFORMAT = "yyyy-MM-dd HH:mm:ss.fff zzz";
		public static string ToDateString(this DateTimeOffset value)
		{
			return value.ToString(FILECONTENT_LASTEXEC_DATEFORMAT);
		}
	}
}
