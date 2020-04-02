using System;
using System.Collections.Generic;
using GrpcCache.Models;

namespace GrpcCache.Service.Providers
{
    public interface ICacheMemory
    {
        Dictionary<int, long> CacheLength { get; }
        ServiceResponse Get(int provider, string key);
        IEnumerable<ServiceResponse> GetAll(int provider, string startKey);
        void AddOrUpdate(int provider, string key, object item, DateTime expireDate);
        void Remove(int provider, string key);
        void RemoveAll(int provider, string startKey);
        void RemoveAll(int provider);
        bool Exists(int provider, string key);
    }

}