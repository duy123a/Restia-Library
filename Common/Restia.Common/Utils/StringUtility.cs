using System;

namespace Restia.Common.Utils
{
	public class StringUtility
	{
		public static string ToEmpty(object objSrc)
		{
			if ((objSrc == null) || (objSrc == DBNull.Value))
			{
				return string.Empty;
			}

			return objSrc.ToString() ?? string.Empty;
		}

		public static string? ToNull(object objSrc)
		{
			return (ToEmpty(objSrc) == string.Empty) ? null : ToEmpty(objSrc);
		}
	}
}
