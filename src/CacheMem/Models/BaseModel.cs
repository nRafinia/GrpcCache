namespace CacheMem.Models
{
    public class BaseModel
    {
        public int Provider { get; set; }
        public string Key { get; set; }
        public bool CheckAll { get; set; } = true;

    }
}