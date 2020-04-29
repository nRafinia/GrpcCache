using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GrpcCache.Service.Extensions;
using NLog;
using GrpcCache.Models;
using ProtoBuf;
using Utf8Json;

namespace GrpcCache.Service.Providers
{
    public class CacheMemoryContract : ICacheMemoryContract
    {
        private readonly ICacheMemory _cacheMem;
        private static readonly Logger ErrorLog = LogManager.GetLogger("ExceptionService");

        public CacheMemoryContract(ICacheMemory cacheMemory)
        {
            _cacheMem = cacheMemory;
        }

        #region Services
        public void AddOrUpdate(AddOrUpdateModel model)
        {
            if (model?.Data == null)
                return;

            try
            {
                model.Key = $"{model.Provider}-{model.Key}";
                var value = new ServiceResponse()
                {
                    Data = model.Data,
                    ExpireDate = model.ExpireDate
                };

                _cacheMem.AddOrUpdate(model.Provider, model.Key, value, model.ExpireDate);
            }
            catch (Exception e)
            {
                ErrorLog.Error(e);
            }
        }

        public BaseServiceResponse Get(BaseModel model)
        {
            ServiceResponse res = null;
            try
            {
                model.Key = $"{model.Provider}-{model.Key}";

                res = _cacheMem.Get(model.Provider, model.Key);
                if (res != null)
                {
                    //var r = JsonConvert.DeserializeObject<ServiceResponse>(res);
                    return res.ExpireDate >= DateTime.Now
                        ? res.ToBase()
                        : new BaseServiceResponse() { Data = null };
                }

            }
            catch (Exception e)
            {
                ErrorLog.Error(e);
            }

            return res == null
                ? new BaseServiceResponse() { Data = null }
                : res.ToBase(); //JsonConvert.DeserializeObject<ServiceResponse>(res);
        }

        public GetAllServiceResponse GetAll(BaseModel model)
        {
            try
            {
                model.Key = $"{model.Provider}-{model.Key}";

                var res = _cacheMem.GetAll(model.Provider, model.Key) ?? new ServiceResponse[0];

                if (!res.Any())
                    return new GetAllServiceResponse() { Data = null };

                var dt = res.Select(r => r?.Data);

                /*using var memoryStream = new MemoryStream();
                Serializer.Serialize(memoryStream, dt);
                return new BaseServiceResponse()
                {
                    Data = memoryStream.ToArray()
                };  */

                return new GetAllServiceResponse()
                {
                    Data = dt,
                    ExpireDate = DateTime.Now.AddYears(1)
                };
            }
            catch (Exception e)
            {
                ErrorLog.Error(e);
            }

            return new GetAllServiceResponse() { Data = null };
        }

        public void Remove(BaseModel model)
        {
            try
            {
                model.Key = $"{model.Provider}-{model.Key}";
                _cacheMem.Remove(model.Provider, model.Key);
            }
            catch (Exception e)
            {
                ErrorLog.Error(e);
            }
        }

        public void RemoveAllWithKey(BaseModel model)
        {
            try
            {
                model.Key = $"{model.Provider}-{model.Key}";
                _cacheMem.RemoveAll(model.Provider, model.Key);
            }
            catch (Exception e)
            {
                ErrorLog.Error(e);
            }
        }

        public void RemoveAll(StructModel<int> provider)
        {
            try
            {
                _cacheMem.RemoveAll(provider.Item);
            }
            catch (Exception e)
            {
                ErrorLog.Error(e);
            }
        }

        public StructModel<bool> Exists(BaseModel model)
        {
            var res = false;
            try
            {
                model.Key = $"{model.Provider}-{model.Key}";

                res = _cacheMem.Exists(model.Provider, model.Key);
                if (res)
                    return new StructModel<bool>() { Item = true };

            }

            catch (Exception e)
            {
                ErrorLog.Error(e);
            }

            return new StructModel<bool>() { Item = res };
        }

        #endregion

        #region Configs

        public Dictionary<string, string> GetStatus()
        {
            var cLength = _cacheMem.CacheLength;
            var res = cLength.ToDictionary(c => $"Provider {c.Key}", c => c.Value.ToString());

            return res;
        }

        #endregion
    }
}