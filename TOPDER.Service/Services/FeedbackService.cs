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
using TOPDER.Service.Dtos.CategoryMenu;
using TOPDER.Service.Dtos.Contact;
using TOPDER.Service.Dtos.Feedback;
using TOPDER.Service.IServices;
using TOPDER.Service.Utils;

namespace TOPDER.Service.Services
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IMapper _mapper;
        private readonly IFeedbackRepository _feedbackRepository;
        public FeedbackService(IFeedbackRepository feedbackRepository, IMapper mapper) { 
            _feedbackRepository = feedbackRepository;
            _mapper = mapper;
        }

        public async Task<bool> AddAsync(FeedbackDto feedbackDto)
        {
            feedbackDto.FeedbackId = 0;
            var feedback = _mapper.Map<Feedback>(feedbackDto);
            return await _feedbackRepository.CreateAsync(feedback);
        }

        public async Task<PaginatedList<FeedbackAdminDto>> GetAdminPagingAsync(int pageNumber, int pageSize)
        {
            var query = await _feedbackRepository.QueryableAsync();

            var feedbacks = query.OrderByDescending(x => x.FeedbackId);

            var queryDTO = feedbacks.Select(r => _mapper.Map<FeedbackAdminDto>(r));

            var paginatedDTOs = await PaginatedList<FeedbackAdminDto>.CreateAsync(
                queryDTO.AsNoTracking(),
                pageNumber > 0 ? pageNumber : 1,
                pageSize > 0 ? pageSize : 10
            );
            return paginatedDTOs;
        }

        public async Task<PaginatedList<FeedbackCustomerDto>> GetCustomerPagingAsync(int pageNumber, int pageSize, int restaurantId)
        {
            var query = await _feedbackRepository.QueryableAsync();

            var feedbacks = query.OrderByDescending(x => x.FeedbackId);

            var queryDTO = feedbacks.Select(r => _mapper.Map<FeedbackCustomerDto>(r));

            var paginatedDTOs = await PaginatedList<FeedbackCustomerDto>.CreateAsync(
                queryDTO.AsNoTracking(),
                pageNumber > 0 ? pageNumber : 1,
                pageSize > 0 ? pageSize : 10
            );
            return paginatedDTOs;
        }

        public async Task<PaginatedList<FeedbackHistoryDto>> GetHistoryCustomerPagingAsync(int pageNumber, int pageSize, int customerId)
        {
            var query = await _feedbackRepository.QueryableAsync();

            var feedbacks = query.OrderByDescending(x => x.FeedbackId);

            var queryDTO = feedbacks.Select(r => _mapper.Map<FeedbackHistoryDto>(r));

            var paginatedDTOs = await PaginatedList<FeedbackHistoryDto>.CreateAsync(
                queryDTO.AsNoTracking(),
                pageNumber > 0 ? pageNumber : 1,
                pageSize > 0 ? pageSize : 10
            );
            return paginatedDTOs;
        }


        public async Task<PaginatedList<FeedbackRestaurantDto>> GetRestaurantPagingAsync(int pageNumber, int pageSize, int restaurantId)
        {
            var query = await _feedbackRepository.QueryableAsync();

            var feedbacks = query.OrderByDescending(x => x.FeedbackId);

            var queryDTO = feedbacks.Select(r => _mapper.Map<FeedbackRestaurantDto>(r));

            var paginatedDTOs = await PaginatedList<FeedbackRestaurantDto>.CreateAsync(
                queryDTO.AsNoTracking(),
                pageNumber > 0 ? pageNumber : 1,
                pageSize > 0 ? pageSize : 10
            );
            return paginatedDTOs;
        }

        public async Task<PaginatedList<FeedbackCustomerDto>> SearchCustomerPagingAsync(int pageNumber, int pageSize, int restaurantId, int star)
        {
            var query = await _feedbackRepository.QueryableAsync();

            var feedbacks = query.Where(x => x.Star == star).OrderByDescending(x => x.FeedbackId);

            var queryDTO = feedbacks.Select(r => _mapper.Map<FeedbackCustomerDto>(r));

            var paginatedDTOs = await PaginatedList<FeedbackCustomerDto>.CreateAsync(
                queryDTO.AsNoTracking(),
                pageNumber > 0 ? pageNumber : 1,
                pageSize > 0 ? pageSize : 10
            );
            return paginatedDTOs;
        }

        public async Task<bool> RemoveAsync(int id)
        {
            var feedback = await _feedbackRepository.GetByIdAsync(id);
            if (feedback == null)
            {
                return false;
            }
            var result = await _feedbackRepository.DeleteAsync(id);
            return result;
        }

        public async Task<PaginatedList<FeedbackAdminDto>> SearchAdminPagingAsync(int pageNumber, int pageSize, int star)
        {
            var query = await _feedbackRepository.QueryableAsync();

            var feedbacks = query.Where(x => x.Star == star).OrderByDescending(x => x.FeedbackId);

            var queryDTO = feedbacks.Select(r => _mapper.Map<FeedbackAdminDto>(r));

            var paginatedDTOs = await PaginatedList<FeedbackAdminDto>.CreateAsync(
                queryDTO.AsNoTracking(),
                pageNumber > 0 ? pageNumber : 1,
                pageSize > 0 ? pageSize : 10
            );
            return paginatedDTOs;
        }

        public async Task<PaginatedList<FeedbackRestaurantDto>> SearchRestaurantPagingAsync(int pageNumber, int pageSize, int restaurantId, int star)
        {
            var query = await _feedbackRepository.QueryableAsync();

            var feedbacks = query.Where(x => x.Star == star).OrderByDescending(x => x.FeedbackId);

            var queryDTO = feedbacks.Select(r => _mapper.Map<FeedbackRestaurantDto>(r));

            var paginatedDTOs = await PaginatedList<FeedbackRestaurantDto>.CreateAsync(
                queryDTO.AsNoTracking(),
                pageNumber > 0 ? pageNumber : 1,
                pageSize > 0 ? pageSize : 10
            );
            return paginatedDTOs;
        }

        public async Task<bool> UpdateAsync(FeedbackDto feedbackDto)
        {
            var existingFeedback = await _feedbackRepository.GetByIdAsync(feedbackDto.FeedbackId);
            if (existingFeedback == null)
            {
                return false;
            }
            var feedback = _mapper.Map<Feedback>(feedbackDto);
            return await _feedbackRepository.UpdateAsync(feedback);
        }
    }
}
