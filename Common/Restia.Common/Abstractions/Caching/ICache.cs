using System;

namespace Restia.Common.Abstractions.Caching
{
	public interface ICache<TKey, TValue>
		where TKey : notnull
		where TValue : class
	{
		bool TryAdd(TKey key, Func<TValue> valueFactory);

		bool TryGetValue(TKey key, out TValue? value);

		TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory);
	}
}
