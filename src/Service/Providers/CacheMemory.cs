using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using nCache.Service.Extensions;
using Microsoft.Extensions.Caching.Memory;
using nCache.Models;

namespace nCache.Service.Providers
{
    public class CacheMemory : ICacheMemory
    {
        private readonly ConcurrentDictionary<int, MyCache> _cache = new ConcurrentDictionary<int, MyCache>();

        public Dictionary<int, long> CacheLength => _cache.ToDictionary(c => c.Key, c => (long)c.Value.Count);

        public ServiceResponse Get(int provider, string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return null;

            var cache = _cache.GetOrAdd(provider, p => new MyCache());
            var res = cache.Get(key) as ServiceResponse;

            return res;
        }

        public IEnumerable<ServiceResponse> GetAll(int provider, string startKey)
        {
            if (string.IsNullOrWhiteSpace(startKey))
                return null;

            var cache = _cache.GetOrAdd(provider, p => new MyCache());
            var res = cache
                .ToList()
                .Where(k => k.Key.StartsWith(startKey))
                .Select(k => k.Value as ServiceResponse);

            return res.Where(k => k?.Data != null);
        }

        public void AddOrUpdate(int provider, string key, object item, DateTime expireDate)
        {
            var cache = _cache.GetOrAdd(provider, p => new MyCache());
            cache.Set(key, item, expireDate);
        }

        public void Remove(int provider, string key)
        {
            var cache = _cache.GetOrAdd(provider, p => new MyCache());
            cache.Remove(key);
        }

        public void RemoveAll(int provider, string startKey)
        {
            var cache = _cache.GetOrAdd(provider, p => new MyCache());
            var lst = cache.GetKeys().Where(k => k.StartsWith(startKey));
            foreach (var item in lst)
            {
                cache.Remove(item);
            }
        }

        public void RemoveAll(int provider)
        {
            var cache = _cache.GetOrAdd(provider, p => new MyCache());
            var items = cache.GetKeys();
            foreach (var item in items)
            {
                cache.Remove(item);
            }
        }

        public bool Exists(int provider, string key)
        {
            return Get(provider, key) != null;
        }

        private class MyCache : MemoryCache
        {
            public MyCache() : base(new MemoryCacheOptions())
            {
            }
        }
    }

}