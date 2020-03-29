using System.Collections.Generic;
using System.Threading.Tasks;

namespace nCache.Client
{
    /// <summary>
    /// nCache client class<br/>
    /// Server url must added in AppSetting.json <example>"CacheMemory":"http://localhost:8050/"</example>
    /// </summary>
    public interface ICacheMemory
    {
        /// <summary>
        /// Add item to cache server
        /// </summary>
        /// <typeparam name="T">Item class type</typeparam>
        /// <param name="key">Key of item, Saved format is <code>typeof(T).FullName:Key</code></param>
        /// <param name="data">data to add</param>
        /// <param name="time">Cache storage time</param>
        /// <param name="provider">Provider id to separate values in different systems</param>
        void AddOrUpdate<T>(string key, T data, CacheTime time = CacheTime.Normal, int provider = 0);

        /// <summary>
        /// Add item to cache server
        /// </summary>
        /// <typeparam name="T">Item class type</typeparam>
        /// <param name="key">Key of item, Saved format is <code>typeof(T).FullName:Key</code></param>
        /// <param name="data">data to add</param>
        /// <param name="time">Cache storage time</param>
        /// <param name="provider">Provider id to separate values in different systems</param>
        void AddOrUpdate<T>(string key, T data, int time, int provider = 0);

        /// <summary>
        /// Get stored item from cache
        /// </summary>
        /// <typeparam name="T">Item class type for cast to</typeparam>
        /// <param name="key">Key of item, Saved format is <code>typeof(T).FullName:Key</code></param>
        /// <param name="provider">Provider id to separate values in different systems</param>
        /// <returns>Returns a value if it exists, otherwise null is returned</returns>
        T Get<T>(string key, int provider = 0);

        /// <summary>
        /// Get all items started with key
        /// </summary>
        /// <typeparam name="T">Item class type for cast to</typeparam>
        /// <param name="key">Start key of item</param>
        /// <param name="provider">Provider id to separate values in different systems</param>
        /// <returns>Return list of exists items</returns>
        IEnumerable<T> GetAll<T>(string key, int provider = 0);

        /// <summary>
        /// Remove item from cache
        /// </summary>
        /// <typeparam name="T">Item class type for cast to</typeparam>
        /// <param name="key">Key of item, Saved format is <code>typeof(T).FullName:Key</code></param>
        /// <param name="provider">Provider id to separate values in different systems</param>
        void Remove<T>(string key, int provider = 0);

        /// <summary>
        /// Remove all items started with key
        /// </summary>
        /// <typeparam name="T">Item class type</typeparam>
        /// <param name="startKey">Start key of item</param>
        /// <param name="provider">Provider id to separate values in different systems</param>
        void RemoveAll<T>(string startKey = "", int provider = 0);

        /// <summary>
        /// Remove all items
        /// </summary>
        /// <param name="provider">Provider id to separate values in different systems</param>
        void RemoveAll(int provider = 0);

        /// <summary>
        /// Check exists item is in cache
        /// </summary>
        /// <typeparam name="T">Item class type</typeparam>
        /// <param name="key">Key of item, Saved format is <code>typeof(T).FullName:Key</code></param>
        /// <param name="provider">Provider id to separate values in different systems</param>
        /// <returns>If exists return True</returns>
        bool Exists<T>(string key, int provider = 0);

        /// <summary>
        /// Get status of cached items
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetStatus();
    }
}