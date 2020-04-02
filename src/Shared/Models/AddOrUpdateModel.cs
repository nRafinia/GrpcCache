using System;
using System.Runtime.Serialization;

namespace GrpcCache.Models
{
    [DataContract]
    public class AddOrUpdateModel 
    {
        [DataMember(Order = 1)]
        public int Provider { get; set; }

        [DataMember(Order = 2)]
        public string Key { get; set; }

        [DataMember(Order = 3)]
        public byte[] Data { get; set; }

        [DataMember(Order = 4)]
        public DateTime ExpireDate { get; set; }
    }
}