using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using CacheMem.Extensions;
using CacheMem.Models;
using NLog;

namespace CacheMem.Providers
{
    public class CacheMemoryContract : ICacheMemoryContract
    {
        private static readonly CacheMemory CacheMem = new CacheMemory();
        private static readonly Logger ErrorLog = LogManager.GetLogger("ExceptionService");

        #region Services
        public void AddOrUpdate(AddOrUpdateModel model)
        {
            if (model?.Item == null)
                return;

            try
            {
                model.Key = $"{model.Provider}-{model.Key}";
                var value = new ServiceResponse()
                {
                    Data = model.Item,
                    ExpireDate = model.ExpireDate
                };

                CacheMem.AddOrUpdate(model.Provider, model.Key, value, model.ExpireDate);
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

                res = CacheMem.Get(model.Provider, model.Key);
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

        public BaseServiceResponse GetAll(BaseModel model)
        {
            IEnumerable<ServiceResponse> res = null;
            try
            {
                model.Key = $"{model.Provider}-{model.Key}";

                res = CacheMem.GetAll(model.Provider, model.Key);
                if (res != null)
                {
                    //var r = JsonConvert.DeserializeObject<ServiceResponse>(res);
                    var raRes = res as ServiceResponse[] ?? res.ToArray();
                    var dt = raRes.Select(r => r?.Data);

                    return raRes.Any()
                        ? new BaseServiceResponse()
                        {
                            Data = JsonSerializer.Serialize(dt)
                        }
                        : new BaseServiceResponse() { Data = null };
                }


            }
            catch (Exception e)
            {
                ErrorLog.Error(e);
            }

            return res == null
                ? new BaseServiceResponse() { Data = null }
                : new BaseServiceResponse()
                {
                    Data = JsonSerializer.Serialize(res.Select(r => r.Data))
                };
        }

        public void Remove(BaseModel model)
        {
            try
            {
                model.Key = $"{model.Provider}-{model.Key}";
                CacheMem.Remove(model.Provider, model.Key);
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
                CacheMem.RemoveAll(model.Provider, model.Key);
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
                CacheMem.RemoveAll(provider.Item);
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
                res = CacheMem.Exists(model.Provider, model.Key);
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
            var cLength = CacheMem.CacheLength;
            var res = cLength.ToDictionary(c => $"Provider {c.Key}", c => c.Value.ToString());

            return res;
        }

        #endregion
    }
}