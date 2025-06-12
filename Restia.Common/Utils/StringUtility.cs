using System;
using System.Text.RegularExpressions;

namespace Restia.Common.Utils
{
    public static class StringUtility
    {
        public static string ToEmpty(object objSrc, string defaultValue = null)
        {
            if ((objSrc == null) || (objSrc == DBNull.Value))
            {
                return defaultValue ?? string.Empty;
            }

            return objSrc.ToString() ?? defaultValue ?? string.Empty;
        }

        public static string ToNull(object objSrc)
        {
            if (objSrc == null || objSrc == DBNull.Value)
                return null;

            string str = objSrc.ToString()?.Trim() ?? "";
            return string.IsNullOrEmpty(str) ? null : str;
        }

        /// <summary>
        /// Create replace message
        /// </summary>
        /// <param name="messageTemplate">Message template</param>
        /// <param name="propertyValues">Property values</param>
        /// <returns>Replacing string</returns>
        /// <exception cref="ArgumentException">The number of property values ​​does not match the number of substitutions in message template.</exception>
        /// <example><code>
        /// StringUtility.CreateReplaceMessage("Processed {RecordCount} records in {TimeMS}.", records.Length, sw.ElapsedMilliseconds);
        /// </code></example>
        public static string CreateReplaceMessage(string messageTemplate, params object[] propertyValues)
        {
            if (propertyValues.Length == 0)
            {
                return messageTemplate;
            }

            var regex = new Regex(@"\{([^{}]+)\}");
            var matches = regex.Matches(messageTemplate);

            if (matches.Count != propertyValues.Length)
            {
                throw new ArgumentException("The number of property values does not match the number of substitutions in message template.");
            }

            for (int i = 0; i < matches.Count; i++)
            {
                var match = matches[i];
                var propertyValue = ToEmpty(propertyValues[i]);
                messageTemplate = messageTemplate.Replace(match.Value, propertyValue);
            }

            return messageTemplate;
        }

        public static string AddSpacesBetweenUpperCharacter(string message)
        {
            var regex = new Regex("(\\B[A-Z])");
            var result = regex.Replace(message, " $1");
            return result.Trim();
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static string ToStringWithTwoDecimals(decimal value)
        {
            return Math.Round(value, 2).ToString("#,##0.00");
        }
    }
}
