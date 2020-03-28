using System;
using System.Runtime.Serialization;

namespace nCache.Models
{
    [DataContract]
    public class BaseServiceResponse
    {
        [DataMember(Order = 1)]
        public byte[] Data { get; set; }

        [DataMember(Order = 2)]
        public DateTime ExpireDate { get; set; }
    }

    [Serializable]
    public class ServiceResponse
    {
        public byte[] Data { get; set; }
        public DateTime ExpireDate { get; set; }
    }
}