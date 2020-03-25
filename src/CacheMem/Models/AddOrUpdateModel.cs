using System;

namespace CacheMem.Models
{
    public class AddOrUpdateModel : BaseModel
    {
        public string Item { get; set; }
        public DateTime ExpireDate { get; set; }
    }
}