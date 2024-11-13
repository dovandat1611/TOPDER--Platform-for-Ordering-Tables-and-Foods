using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Repository.Entities;
using TOPDER.Repository.IRepositories;
using TOPDER.Service.Dtos.AdvertisementPricing;
using TOPDER.Service.Dtos.Blog;
using TOPDER.Service.IServices;
using static TOPDER.Service.Common.ServiceDefinitions.Constants;

namespace TOPDER.Service.Services
{
    public class AdvertisementPricingService : IAdvertisementPricingService
    {
        private readonly IMapper _mapper;
        private readonly IAdvertisementPricingRepository _advertisementPricingRepository;

        public AdvertisementPricingService(IAdvertisementPricingRepository advertisementPricingRepository, IMapper mapper)
        {
            _advertisementPricingRepository = advertisementPricingRepository;
            _mapper = mapper;
        }

        public async Task<bool> AddAsync(AdvertisementPricingDto advertisementPricingDto)
        {
            var advertisementPricing = _mapper.Map<AdvertisementPricing>(advertisementPricingDto);
            return await _advertisementPricingRepository.CreateAsync(advertisementPricing);
        }

        public async Task<List<AdvertisementPricingDto>> GetAllAdvertisementPricingAsync()
        {
            var advertisementPricing = await _advertisementPricingRepository.GetAllAsync();

            var advertisementPricingDto = _mapper.Map<List<AdvertisementPricingDto>>(advertisementPricing);

            return advertisementPricingDto;
        }

        public async Task<bool> RemoveAsync(int id)
        {
            var advertisementPricing = await _advertisementPricingRepository.GetByIdAsync(id);

            if(advertisementPricing == null)
            {
                return false;
            }
            return await _advertisementPricingRepository.DeleteAsync(id);
        }

        public async Task<bool> UpdateAsync(AdvertisementPricingDto advertisementPricing)
        {
            var existingAdvertisementPricing = await _advertisementPricingRepository.GetByIdAsync(advertisementPricing.PricingId);
            if (existingAdvertisementPricing == null)
            {
                return false;
            }
            existingAdvertisementPricing.DurationHours = advertisementPricing.DurationHours;
            existingAdvertisementPricing.Description = advertisementPricing.Description;
            existingAdvertisementPricing.Price = existingAdvertisementPricing.Price;
            existingAdvertisementPricing.PricingName = existingAdvertisementPricing.PricingName;
            return await _advertisementPricingRepository.UpdateAsync(existingAdvertisementPricing);
        }
    }
}
