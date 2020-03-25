using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using CacheMem.Extensions;
using CacheMem.Models;
using CacheMem.Providers;
using Microsoft.AspNetCore.Mvc;

namespace CacheMem.Controllers
{
    [Route("CacheMem")]
    public class RestController : Controller
    {
        private static readonly Logger ErrorLog = LogManager.GetLogger("ExceptionService");

        private readonly ICacheMemory _cacheMemory;
        public RestController(ICacheMemory cacheMemory)
        {
            _cacheMemory = cacheMemory;
        }

        #region Services

        [HttpPost("AddOrUpdate")]
        public void AddOrUpdate([FromBody] AddOrUpdateModel model)
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

                _cacheMemory.AddOrUpdate(model.Provider, model.Key, value, model.ExpireDate);
            }
            catch (Exception e)
            {
                ErrorLog.Error(e);
            }
        }

        [HttpPost("Get")]
        public BaseServiceResponse Get([FromBody] BaseModel model)
        {
            ServiceResponse res = null;
            try
            {
                model.Key = $"{model.Provider}-{model.Key}";

                res = _cacheMemory.Get(model.Provider, model.Key);
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

        [Route("GetAll")]
        [HttpPost]
        public BaseServiceResponse GetAll([FromBody] BaseModel model)
        {
            IEnumerable<ServiceResponse> res = null;
            try
            {
                model.Key = $"{model.Provider}-{model.Key}";

                res = _cacheMemory.GetAll(model.Provider, model.Key);
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

        [Route("Remove")]
        [HttpPost]
        public void Remove([FromBody] BaseModel model)
        {
            try
            {
                model.Key = $"{model.Provider}-{model.Key}";
                _cacheMemory.Remove(model.Provider, model.Key);
            }
            catch (Exception e)
            {
                ErrorLog.Error(e);
            }
        }

        [Route("RemoveAllWithKey")]
        [HttpPost]
        public void RemoveAllWithKey([FromBody] BaseModel model)
        {
            try
            {
                model.Key = $"{model.Provider}-{model.Key}";
                _cacheMemory.RemoveAll(model.Provider, model.Key);
            }
            catch (Exception e)
            {
                ErrorLog.Error(e);
            }
        }

        [Route("RemoveAll/{provider}")]
        [HttpGet]
        public void RemoveAll(int provider)
        {
            try
            {
                _cacheMemory.RemoveAll(provider);
            }
            catch (Exception e)
            {
                ErrorLog.Error(e);
            }
        }

        [Route("Exists")]
        [HttpPost]
        public bool Exists([FromBody] BaseModel model)
        {
            var res = false;
            try
            {
                res = _cacheMemory.Exists(model.Provider, model.Key);
                if (res)
                    return true;

            }

            catch (Exception e)
            {
                ErrorLog.Error(e);
            }

            return res;
        }

        #endregion

        #region Configs

        [HttpGet]
        [Route("GetStatus")]
        public Dictionary<string, string> GetStatus()
        {
            var cLength = _cacheMemory.CacheLength;
            var res = cLength.ToDictionary(c => $"Provider {c.Key}", c => c.Value.ToString());

            return res;
        }

        [HttpGet]
        [Route("SaveCacheMemoryToDisk")]
        public bool SaveCacheMemoryToDisk()
        {
            return true;
        }

        #endregion
    }
}