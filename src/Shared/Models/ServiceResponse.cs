using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GrpcCache.Models
{
    [DataContract]
    public class BaseServiceResponse
    {
        [DataMember(Order = 1)]
        public byte[] Data { get; set; }

        [DataMember(Order = 2)]
        public DateTime ExpireDate { get; set; }
    }

    [DataContract]
    public class ServiceResponse
    {
        [DataMember(Order = 1)]
        public byte[] Data { get; set; }

        [DataMember(Order = 2)]
        public DateTime ExpireDate { get; set; }
    }    
    
    [DataContract]
    public class GetAllServiceResponse
    {
        [DataMember(Order = 1)]
        public IEnumerable<byte[]> Data { get; set; }

        [DataMember(Order = 2)]
        public DateTime ExpireDate { get; set; }
    }
}