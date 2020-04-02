using GrpcCache.Models;

namespace GrpcCache.Service.Extensions
{
    public static class Extensions
    {
        public static BaseServiceResponse ToBase(this ServiceResponse data)
        {
            return new BaseServiceResponse()
            {
                Data = data.Data,
                ExpireDate = data.ExpireDate
            };
        }
    }
}