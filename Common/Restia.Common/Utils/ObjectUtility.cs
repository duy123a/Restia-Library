using System;
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
	}
}
