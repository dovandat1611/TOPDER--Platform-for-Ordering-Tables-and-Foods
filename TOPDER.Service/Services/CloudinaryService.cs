using Microsoft.AspNetCore.Http;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TOPDER.Service.IServices;

namespace Service.Services
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(IConfiguration configuration)
        {
            var account = new Account(
                configuration["Cloudinary:CloudName"],
                configuration["Cloudinary:ApiKey"],
                configuration["Cloudinary:ApiSecret"]
            );
            _cloudinary = new Cloudinary(account);
        }

        public async Task<List<ImageUploadResult>> UploadImagesAsync(List<IFormFile> files)
        {
            var uploadResults = new List<ImageUploadResult>();

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    using (var stream = file.OpenReadStream())
                    {
                        var uploadParams = new ImageUploadParams()
                        {
                            File = new FileDescription(file.FileName, stream)
                        };
                        var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                        uploadResults.Add(uploadResult);
                    }
                }
            }
            return uploadResults;
        }
        public async Task<ImageUploadResult?> UploadImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return null; 
            }

            using (var stream = file.OpenReadStream())
            {
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(file.FileName, stream)
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                return uploadResult;
            }
        }
    }
}
