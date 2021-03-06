﻿using System.Collections.Generic;
using System.ServiceModel;
using GrpcCache.Models;

namespace GrpcCache
{
    [ServiceContract(Name = "CacheMemory")]
    public interface ICacheMemoryContract
    {
        void AddOrUpdate(AddOrUpdateModel model);
        BaseServiceResponse Get(BaseModel model);
        GetAllServiceResponse GetAll(BaseModel model);
        void Remove(BaseModel model);
        void RemoveAllWithKey(BaseModel model);
        void RemoveAll(StructModel<int> provider);
        StructModel<bool> Exists(BaseModel model);
        Dictionary<string, string> GetStatus();
        //bool SaveCacheMemoryToDisk();
    }
}