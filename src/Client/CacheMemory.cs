using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using ProtoBuf.Grpc.Client;
using GrpcCache.Models;
using ProtoBuf;

namespace GrpcCache.Client
{
    /// <inheritdoc />
    public class CacheMemory : ICacheMemory
    {
        private ICacheMemoryContract _cache;
        public CacheMemory(IConfiguration config)
        {
            GrpcClientFactory.AllowUnencryptedHttp2 = true;

            if (config == null)
                return;

            var cacheUrl = config["CacheMemory"];
            GetCache(cacheUrl);
        }

        public static CacheMemory GetInstance(string cacheUrl)
        {
            var res = new CacheMemory(null);
            res.GetCache(cacheUrl);
            return res;
        }

        /// <inheritdoc />
        public T Get<T>(string key, int provider = 0)
        {
            var res = default(T);
            try
            {
                if (string.IsNullOrWhiteSpace(key))
                    return default;

                key = GetKeyName(key, typeof(T).FullName);
                var item = _cache.Get(new BaseModel()
                {
                    Key = key,
                    Provider = provider
                });

                if (item == null || item.ExpireDate < DateTime.Now)
                    return default;

                using var memoryStream = new MemoryStream(item.Data);
                res = Serializer.Deserialize<T>(memoryStream);
            }
            catch
            {
                //
            }

            return res == null ? default : res;
        }

        /// <inheritdoc />
        public IEnumerable<T> GetAll<T>(string key, int provider = 0)
        {
            IEnumerable<T> res = null;
            try
            {
                key = GetKeyName(key, typeof(T).FullName);
                var items = _cache.GetAll(new BaseModel()
                {
                    Key = key,
                    Provider = provider
                });

                if (items.Data == null)
                {
                    return new List<T>();
                }

                using var memoryStream = new MemoryStream(items.Data);
                res = Serializer.Deserialize<List<T>>(memoryStream);
            }
            catch
            {
                //
            }

            return res ?? new List<T>();
        }

        /// <inheritdoc />
        public void AddOrUpdate<T>(string key, T data, CacheTime time = CacheTime.Normal, int provider = 0)
        {
            AddOrUpdate(key, data, (int)time, provider);
        }

        /// <inheritdoc />
        public void AddOrUpdate<T>(string key, T data, int time, int provider = 0)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(key) || EqualityComparer<T>.Default.Equals(data, default))
                    return;

                byte[] dataBytes = null;
                SerializerBuilder.Build(data);
                using (var memoryStream = new MemoryStream())
                {
                    Serializer.Serialize(memoryStream, data);
                    dataBytes = memoryStream.ToArray();
                }

                key = GetKeyName(key, typeof(T).FullName);
                _cache.AddOrUpdate(new AddOrUpdateModel()
                {
                    Key = key,
                    Provider = provider,
                    Data = dataBytes,
                    ExpireDate = DateTime.Now.AddSeconds(time)
                });

            }
            catch
            {
                //
            }
        }

        /// <inheritdoc />
        public void Remove<T>(string key, int provider = 0)
        {
            try
            {
                key = GetKeyName(key, typeof(T).FullName);
                _cache.Remove(new BaseModel()
                {
                    Key = key,
                    Provider = provider
                });
            }
            catch
            {
                //
            }
        }

        /// <inheritdoc />
        public void RemoveAll<T>(string startKey = "", int provider = 0)
        {
            try
            {
                _cache.RemoveAllWithKey(new BaseModel()
                {
                    Key = GetKeyName(startKey, typeof(T).FullName),
                    Provider = provider
                });
            }
            catch
            {
                //
            }
        }

        /// <inheritdoc />
        public void RemoveAll(int provider = 0)
        {
            try
            {
                _cache.RemoveAll(new StructModel<int>() { Item = provider });
            }
            catch
            {
                //
            }
        }

        /// <inheritdoc />
        public bool Exists<T>(string key, int provider = 0)
        {
            key = GetKeyName(key, typeof(T).FullName);
            var res = _cache.Exists(new BaseModel()
            {
                Key = key,
                Provider = provider
            });
            return res.Item;
        }

        public Dictionary<string, string> GetStatus()
        {
            return _cache.GetStatus();
        }

        #region Private methods

        private static string GetKeyName(string key, string prefix)
        {
            return $"{prefix}:{key}";
        }

        private void GetCache(string cacheUrl)
        {
            var channel = GrpcChannel.ForAddress(cacheUrl);
            _cache = channel.CreateGrpcService<ICacheMemoryContract>();
        }

        #endregion
    }
}