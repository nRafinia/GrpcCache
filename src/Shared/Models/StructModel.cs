using System.Runtime.Serialization;

namespace GrpcCache.Models
{
    [DataContract]
    public class StructModel<T>
        where T : struct
    {
        [DataMember(Order = 1)]
        public T Item { get; set; }
    }
}