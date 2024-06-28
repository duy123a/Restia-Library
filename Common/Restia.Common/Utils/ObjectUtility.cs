using System;
using System.Globalization;
using System.IO;
using System.Xml.Serialization;

namespace Restia.Common.Utils
{
	public class ObjectUtility
	{
		public static T DeepCopy<T>(T source)
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}

			using (var stream = new MemoryStream())
			{
				var serializer = new XmlSerializer(typeof(T));
				serializer.Serialize(stream, source);
				stream.Position = 0;

				var result = (T)serializer.Deserialize(stream);
				return result == null ? throw new InvalidOperationException("Deserialization returned null.") : result;
			}
		}

		public static bool TryParseBool(string value)
		{
			if (bool.TryParse(value, out var result) == false) return false;
			return result;
		}

		public static int TryParseInt(string value, int defaultValue)
		{
			if (int.TryParse(value, out var result) == false) return defaultValue;
			return result;
		}

		public static int? TryParseInt(string value)
		{
			if (int.TryParse(value, out var result) == false) return null;
			return result;
		}

		public static decimal TryParseDecimal(string value, decimal defaultValue)
		{
			if (decimal.TryParse(value, out var result) == false) return defaultValue;
			return result;
		}

		public static decimal? TryParseDecimal(string value)
		{
			if (decimal.TryParse(value, out var result) == false) return null;
			return result;
		}

		public static DateTime TryParseDateTime(string value, DateTime defaultValue)
		{
			if (DateTime.TryParse(value, out var result) == false) return defaultValue;
			return result;
		}

		public static DateTime? TryParseDateTime(string value)
		{
			if (DateTime.TryParse(value, out var result) == false) return null;
			return result;
		}

		public static DateTime TryParseExacDateTime(string value, string format, DateTime defaultValue)
		{
			if (DateTime.TryParseExact(value, format, null, DateTimeStyles.None, out var result) == false) return defaultValue;
			return result;
		}
	}
}
