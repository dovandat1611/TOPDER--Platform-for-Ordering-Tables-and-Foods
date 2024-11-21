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
using TOPDER.Service.Dtos.Admin;
using TOPDER.Service.Dtos.BookingAdvertisement;
using TOPDER.Service.IServices;
using static TOPDER.Service.Common.ServiceDefinitions.Constants;

namespace TOPDER.Service.Services
{
    public class BookingAdvertisementService : IBookingAdvertisementService
    {
        private readonly IMapper _mapper;
        private readonly IBookingAdvertisementRepository _bookingAdvertisementRepository;
        private readonly IAdvertisementPricingRepository _advertisementPricingRepository;

        public BookingAdvertisementService(IBookingAdvertisementRepository bookingAdvertisementRepository, IMapper mapper, IAdvertisementPricingRepository advertisementPricingRepository)
        {
            _bookingAdvertisementRepository = bookingAdvertisementRepository;
            _mapper = mapper;
            _advertisementPricingRepository = advertisementPricingRepository;
        }
        public async Task<bool> AddAsync(CreateBookingAdvertisementDto bookingAdvertisementDto)
        {
            TimeSpan difference = bookingAdvertisementDto.EndTime - bookingAdvertisementDto.StartTime;
            double totalHours = difference.TotalHours;

            var queryAdvertisementPricing = await _advertisementPricingRepository.QueryableAsync();

            var advertisementPricing = queryAdvertisementPricing
                .Where(x => x.DurationHours <= totalHours)    
                .OrderByDescending(x => x.DurationHours)
                .FirstOrDefault();

            var pricePerHours = (double)(advertisementPricing.Price / advertisementPricing.DurationHours);

            var totalAmount = (decimal)(pricePerHours * totalHours);

            BookingAdvertisement bookingAdvertisement = new BookingAdvertisement() {
                BookingId = 0,
                RestaurantId = bookingAdvertisementDto.RestaurantId,
                StartTime = bookingAdvertisementDto.StartTime,
                EndTime = bookingAdvertisementDto.EndTime,
                Title = bookingAdvertisementDto.Title,
                Status = Common_Status.INACTIVE,
                CreatedAt = DateTime.Now,
                StatusPayment = Payment_Status.PENDING,
                TotalAmount = totalAmount,
            };
            return await _bookingAdvertisementRepository.CreateAsync(bookingAdvertisement);
        }

        public async Task<List<BookingAdvertisementViewDto>> GetAllBookingAdvertisementAvailableAsync()
        {
            var currentTime = DateTime.Now;

            var query = await _bookingAdvertisementRepository.QueryableAsync();

            var bookingAdvertisements = await query.Include(x => x.Restaurant)
                .ThenInclude(x => x.CategoryRestaurant)
                .Where(x => x.StatusPayment == Payment_Status.SUCCESSFUL && x.Status == Common_Status.ACTIVE
                && currentTime >= x.StartTime
                && currentTime <= x.EndTime)
                .ToListAsync();  

            var listBookingAdvertisements = _mapper.Map<List<BookingAdvertisementViewDto>>(bookingAdvertisements);

            return listBookingAdvertisements;
        }


        public async Task<List<BookingAdvertisementDto>> GetAllBookingAdvertisementForRestaurantAsync(int restaurantId)
        {
            var query = await _bookingAdvertisementRepository.QueryableAsync();

            var bookingAdvertisements = await query.Where(x => x.RestaurantId == restaurantId)
                .OrderByDescending(x => x.BookingId)
                .ToListAsync();

            var listBookingAdvertisements = _mapper.Map<List<BookingAdvertisementDto>>(bookingAdvertisements);

            return listBookingAdvertisements;
        }


        public async Task<List<BookingAdvertisementAdminDto>> GetAllBookingAdvertisementForAdminAsync()
        {
            var query = await _bookingAdvertisementRepository.QueryableAsync();

            var bookingAdvertisements = await query.Include(x => x.Restaurant).OrderByDescending(x => x.BookingId).ToListAsync();

            var listBookingAdvertisements = _mapper.Map<List<BookingAdvertisementAdminDto>>(bookingAdvertisements);

            return listBookingAdvertisements;
        }

        public async Task<BookingAdvertisementDto> UpdateStatusAsync(int bookingId, string status)
        {
            var existingAdvertisement = await _bookingAdvertisementRepository.GetByIdAsync(bookingId);

            if (existingAdvertisement == null)
            {
                return null;
            }

            if (status == Booking_Status.INACTIVE || status == Booking_Status.ACTIVE || status == Booking_Status.CANCELLED)
            {
                existingAdvertisement.Status = status;
                await _bookingAdvertisementRepository.UpdateAsync(existingAdvertisement);

                return _mapper.Map<BookingAdvertisementDto>(existingAdvertisement);
            }
            return null;
        }

        public async Task<BookingAdvertisementDto> UpdateStatusPaymentAsync(int bookingId, string status)
        {
            var existingAdvertisement = await _bookingAdvertisementRepository.GetByIdAsync(bookingId);

            if (existingAdvertisement == null)
            {
                return null;
            }

            if (status == Payment_Status.SUCCESSFUL || status == Payment_Status.CANCELLED)
            {
                existingAdvertisement.StatusPayment = status;
                await _bookingAdvertisementRepository.UpdateAsync(existingAdvertisement);

                return _mapper.Map<BookingAdvertisementDto>(existingAdvertisement);
            }
            return null; 
        }



    }
}
