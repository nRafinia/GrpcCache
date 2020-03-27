using System;
using System.Runtime.Serialization;

namespace CacheMem.Models
{
    [DataContract]
    public class BaseServiceResponse
    {
        [DataMember(Order = 1)]
        public string Data { get; set; }

        [DataMember(Order = 2)]
        public DateTime ExpireDate { get; set; }
    }

    [Serializable]
    public class ServiceResponse
    {
        public string Data { get; set; }
        public DateTime ExpireDate { get; set; }
    }
}