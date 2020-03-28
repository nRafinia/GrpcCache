using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.Caching.Memory;

namespace nCache.Service.Extensions
{
    public static class CacheMemoryExt
    {
        public static IEnumerable<string> GetKeys(this MemoryCache cache)
        {
            var field = typeof(MemoryCache).GetProperty("EntriesCollection", BindingFlags.NonPublic | BindingFlags.Instance);
            if (field == null)
                return new List<string>();

            var collection = field.GetValue(cache) as ICollection;
            var items = new List<string>();
            if (collection == null)
                return items;

            foreach (var item in collection)
            {
                var methodInfo = item.GetType().GetProperty("Key");
                if (methodInfo == null)
                    continue;

                var val = methodInfo.GetValue(item);
                items.Add(val.ToString());
            }

            return items;
        }

        public static Dictionary<string, object> ToList(this MemoryCache cache)
        {
            var field = typeof(MemoryCache).GetProperty("EntriesCollection", BindingFlags.NonPublic | BindingFlags.Instance);
            if (field == null)
                return new Dictionary<string, object>();

            var collection = field.GetValue(cache) as ICollection;
            var items = new Dictionary<string, object>();
            if (collection == null)
                return items;

            foreach (var item in collection)
            {
                var methodInfo = item.GetType().GetProperty("Key");
                if (methodInfo == null)
                    continue;

                var val = methodInfo.GetValue(item);

                items.Add(val.ToString(), cache.Get(val.ToString()));
            }

            return items;
        }
    }
}