using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public struct CacheItem<T>
{
    public T Value { get; }
    public DateTime LastAccessed { get; private set; }

    public CacheItem(T value)
    {
        Value = value;
        LastAccessed = DateTime.Now;
    }

    public void UpdateAccessTime()
    {
        LastAccessed = DateTime.Now;
    }
}

public class CacheManager<TKey, TValue>
{
    private Dictionary<TKey, CacheItem<TValue>> _cache = new Dictionary<TKey, CacheItem<TValue>>();
    private TimeSpan _expirationDuration;

    public CacheManager(TimeSpan expirationDuration)
    {
        _expirationDuration = expirationDuration;
    }

    public void Add(TKey key, TValue value)
    {
        var item = new CacheItem<TValue>(value);
        _cache[key] = item;
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        value = default;
        if (_cache.TryGetValue(key, out CacheItem<TValue> item))
        {
            item.UpdateAccessTime();
            _cache[key] = item;
            value = item.Value;
            return true;
        }
        return false;
    }

    public List<CacheItem<TValue>> ToList => _cache.Values.ToList();

    public void RemoveExpiredItems()
    {
        var now = DateTime.Now;
        var keysToRemove = new List<TKey>();
        foreach (var pair in _cache)
        {
            if (now - pair.Value.LastAccessed <= _expirationDuration)
                continue;

            keysToRemove.Add(pair.Key);
        }

        foreach (var key in keysToRemove)
        {
            _cache.Remove(key);
        }
    }

    public void Clear()
    {
        _cache.Clear();
    }
}
