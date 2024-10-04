using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Repository.Entities;
using TOPDER.Repository.IRepositories;
using TOPDER.Repository.Repositories;
using TOPDER.Service.Dtos.BlogGroup;
using TOPDER.Service.Dtos.Image;
using TOPDER.Service.IServices;
using TOPDER.Service.Utils;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace TOPDER.Service.Services
{
    public class ImageService : IImageService
    {
        private readonly IMapper _mapper;
        private readonly IImageRepository _imageRepository;

        public ImageService(IImageRepository imageRepository, IMapper mapper)
        {
            _imageRepository = imageRepository;
            _mapper = mapper;
        }
        public async Task<bool> AddAsync(ImageDto createImageDto)
        {
            var image = _mapper.Map<Image>(createImageDto);
            return await _imageRepository.CreateAsync(image);
        }

        public async Task<bool> AddRangeAsync(List<ImageDto> createImagesDto)
        {
            var images = _mapper.Map<List<Image>>(createImagesDto);
            return await _imageRepository.CreateRangeAsync(images);
        }

        public async Task<ImageDto> GetItemAsync(int imageId, int restaurantId)
        {
            var check = await _imageRepository.GetByIdAsync(imageId);

            if (check == null || check.RestaurantId != restaurantId)
            {
                throw new KeyNotFoundException($"Không tìm thấy hình ảnh với Id {imageId} cho nhà hàng với Id {restaurantId}.");
            }
            var image = _mapper.Map<ImageDto>(check);
            return image;
        }

        public async Task<PaginatedList<ImageDto>> GetPagingAsync(int pageNumber, int pageSize, int restaurantId)
        {
            var queryable = await _imageRepository.QueryableAsync();

            var query = queryable.Where(x => x.RestaurantId == restaurantId);

            var queryDTO = query.Select(r => _mapper.Map<ImageDto>(r));

            var paginatedDTOs = await PaginatedList<ImageDto>.CreateAsync(
                queryDTO.AsNoTracking(),
                pageNumber > 0 ? pageNumber : 1,
                pageSize > 0 ? pageSize : 10
            );
            return paginatedDTOs;
        }

        public async Task<bool> RemoveAsync(int id, int restaurantId)
        {
            var query = await _imageRepository.QueryableAsync();

            var check = query.Where(x => x.ImageId == id && x.RestaurantId == restaurantId);

            if (check == null)
            {
                throw new KeyNotFoundException($"No image with ImageId {id} found for RestaurantId {restaurantId}.");
            }

            return await _imageRepository.DeleteAsync(id);
        }

        public async Task<bool> UpdateAsync(ImageDto imageDto)
        {
            var existingImage = await _imageRepository.GetByIdAsync(imageDto.ImageId);

            if (existingImage == null || existingImage.RestaurantId != imageDto.RestaurantId)
            {
                return false;
            }

            var image = _mapper.Map<Image>(imageDto);
            return await _imageRepository.UpdateAsync(image);
        }

    }
}
