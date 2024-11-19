using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Service.Dtos.AdvertisementPricing;
using TOPDER.Service.Dtos.BlogGroup;

namespace TOPDER.Service.IServices
{
    public interface IAdvertisementPricingService
    {
        Task<bool> AddAsync(AdvertisementPricingDto advertisementPricing);
        Task<bool> UpdateAsync(AdvertisementPricingDto advertisementPricing);
        Task<bool> RemoveAsync(int id);
        Task<List<AdvertisementPricingDto>> GetAllAdvertisementPricingAsync();
    }
}
