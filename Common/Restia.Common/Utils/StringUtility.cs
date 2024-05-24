using System;
using System.Text.RegularExpressions;

namespace Restia.Common.Utils
{
	public class StringUtility
	{
		public static string ToEmpty(object? objSrc)
		{
			if ((objSrc == null) || (objSrc == DBNull.Value))
			{
				return string.Empty;
			}

			return objSrc.ToString() ?? string.Empty;
		}

		public static string? ToNull(object? objSrc)
		{
			return (ToEmpty(objSrc) == string.Empty) ? null : ToEmpty(objSrc);
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
	}
}
