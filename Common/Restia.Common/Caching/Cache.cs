using System;
using System.Collections.Concurrent;

namespace Restia.Common.Caching
{
	public class Cache<TKey, TValue>
		where TKey : notnull
	{
		private readonly ConcurrentDictionary<TKey, Lazy<TValue>> _cache = new ConcurrentDictionary<TKey, Lazy<TValue>>();

		public bool TryAdd(TKey key, Func<TValue> valueFactory) =>
			_cache.TryAdd(key, new Lazy<TValue>(valueFactory));

		public bool TryGetValue(TKey key, out TValue value)
		{
			if (_cache.TryGetValue(key, out var lazyValue))
			{
				value = lazyValue.Value;
				return true;
			}
			else
			{
				value = default!;
				return false;
			}
		}

		public TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory) =>
			_cache.GetOrAdd(key, k => new Lazy<TValue>(() => valueFactory(k))).Value;
	}
}
