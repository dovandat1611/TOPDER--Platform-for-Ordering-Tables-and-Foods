using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Service.Dtos.Image;
using TOPDER.Service.Utils;

namespace TOPDER.Service.IServices
{
    public interface IImageService
    {
        Task<bool> AddAsync(ImageDto createImageDto);
        Task<bool> UpdateAsync(ImageDto imageDto);
        Task<bool> RemoveAsync(int id, int restaurantId);
        Task<ImageDto> GetItemAsync(int id, int restaurantId);
        Task<PaginatedList<ImageDto>> GetPagingAsync(int pageNumber, int pageSize, int restaurantId);
    }
}
