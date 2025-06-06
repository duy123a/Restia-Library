using System;
using System.Globalization;
using System.IO;
using System.Xml.Serialization;

namespace Restia.Common.Utils
{
    public static class ObjectUtility
    {
        /// <summary>
        /// Creates a deep copy of the specified object using XML serialization.
        /// </summary>
        /// <typeparam name="T">The type of the object to copy. Must be serializable by <see cref="XmlSerializer"/>.</typeparam>
        /// <param name="source">The source object to clone.</param>
        /// <returns>A deep copy of the source object.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the source object is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if deserialization fails and returns null.</exception>
        /// <remarks>
        /// This method uses <see cref="System.Xml.Serialization.XmlSerializer"/> to serialize the object to memory and then deserialize it.
        /// All properties and fields must be XML serializable. Types with circular references are not supported.
        /// </remarks>
        public static T DeepCopy<T>(T source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var serializer = new XmlSerializer(typeof(T));
            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, source);
                stream.Position = 0;
                var result = (T)serializer.Deserialize(stream);

                if (result == null)
                    throw new InvalidOperationException("Deserialization returned null.");

                return result;
            }
        }

        public static string Base64UrlEncode(byte[] input)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));

            return Convert.ToBase64String(input)
                .Replace("+", "-")
                .Replace("/", "_")
                .TrimEnd('=');
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

        public static DateTime TryParseExactDateTime(string value, string format, DateTime defaultValue)
        {
            if (DateTime.TryParseExact(value, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result) == false) return defaultValue;
            return result;
        }
    }
}
