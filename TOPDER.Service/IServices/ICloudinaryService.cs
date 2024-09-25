using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.IServices
{
    public interface ICloudinaryService
    {
        Task<ImageUploadResult?> UploadImageAsync(IFormFile file);
        Task<List<ImageUploadResult>> UploadImagesAsync(List<IFormFile> files);
    }
}
