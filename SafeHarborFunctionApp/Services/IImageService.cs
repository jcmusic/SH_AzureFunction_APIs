using System;
using System.Threading.Tasks;

namespace SafeHarborFunctionApp.Services
{
    public interface IImageService
    {
        Task<bool> GetProfilePic(Guid profileId);
    }


    public class ImageService : IImageService
    {
        public Task<bool> GetProfilePic(Guid profileId)
        {
            return Task.FromResult(true);
        }
    }
}
