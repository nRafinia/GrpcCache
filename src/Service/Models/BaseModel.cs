using System.Runtime.Serialization;

namespace CacheMem.Models
{
    [DataContract]
    public class BaseModel
    {
        [DataMember(Order = 1)]
        public int Provider { get; set; }
        
        [DataMember(Order = 2)]
        public string Key { get; set; }

    }
}