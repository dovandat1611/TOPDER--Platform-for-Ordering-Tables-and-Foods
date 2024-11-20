using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Repository.Entities;
using TOPDER.Service.Dtos.AdvertisementPricing;
using TOPDER.Service.Dtos.BookingAdvertisement;

namespace TOPDER.Service.IServices
{
    public interface IBookingAdvertisementService
    {
        Task<bool> AddAsync(CreateBookingAdvertisementDto bookingAdvertisement);
        Task<List<BookingAdvertisementDto>> GetAllBookingAdvertisementForRestaurantAsync(int restaurantId);
        Task<List<BookingAdvertisementViewDto>> GetAllBookingAdvertisementAvailableAsync();
        Task<List<BookingAdvertisementAdminDto>> GetAllBookingAdvertisementForAdminAsync();
        Task<BookingAdvertisementDto> UpdateStatusAsync(int bookingId, string status);
        Task<bool> UpdateStatusPaymentAsync(int bookingId, string status);

    }
}
