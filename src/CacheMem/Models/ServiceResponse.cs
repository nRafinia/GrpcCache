using System;

namespace CacheMem.Models
{
    public class BaseServiceResponse
    {
        public string Data { get; set; }
        public DateTime ExpireDate { get; set; }
    }

    [Serializable]
    public class ServiceResponse
    {
        public string Data { get; set; }
        public DateTime ExpireDate { get; set; }
    }
}