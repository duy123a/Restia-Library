using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using AttributeCache = Restia.Common.Caching.Cache<(System.Type, System.Type), System.Attribute>;
using PropertyCache = Restia.Common.Caching.Cache<System.Type, System.Collections.Generic.Dictionary<string, System.Reflection.PropertyInfo>>;

namespace Restia.Common.Extensions
{
	public static class ObjectExtension
	{
		private static readonly AttributeCache _attributeCache = new AttributeCache();
		private static readonly PropertyCache _propertyCache = new PropertyCache();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static object? GetPropertyValue(this object obj, string name) =>
			GetProperty(obj.GetType(), name).GetValue(obj);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void SetPropertyValue(this object obj, string name, object value) =>
			GetProperty(obj.GetType(), name).SetValue(obj, value);

		private static PropertyInfo GetProperty(Type type, string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException(nameof(name));
			}

			// Caching PropertyInfo for quick lookup instead of using reflection each time.
			if (!_propertyCache.TryGetValue(type, out var properties))
			{
				properties = type.GetProperties().ToDictionary(p => p.Name, p => p);
				_propertyCache.TryAdd(type, () => properties);
			}

			if (!properties.TryGetValue(name, out var property))
			{
				throw new ArgumentException(
					$"The object of type {type.Name} doesn't have a property named {name}",
					nameof(name));
			}

			return property;
		}
	}
}
