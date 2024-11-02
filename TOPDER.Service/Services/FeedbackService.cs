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
using static TOPDER.Service.Common.ServiceDefinitions.Constants;

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
            feedback.CreateDate = DateTime.Now;
            feedback.Status = Common_Status.ACTIVE;
            return await _feedbackRepository.CreateAsync(feedback);
        }

        public async Task<PaginatedList<FeedbackHistoryDto>> GetHistoryCustomerPagingAsync(int pageNumber, int pageSize, int customerId)
        {
            var query = await _feedbackRepository.QueryableAsync();

            var feedbacks = query
                .Include(x => x.Restaurant)
                .OrderByDescending(x => x.FeedbackId)
                .Where(x => x.CustomerId == customerId);

            var queryDTO = feedbacks.Select(r => _mapper.Map<FeedbackHistoryDto>(r));

            var paginatedDTOs = await PaginatedList<FeedbackHistoryDto>.CreateAsync(
                queryDTO.AsNoTracking(),
                pageNumber > 0 ? pageNumber : 1,
                pageSize > 0 ? pageSize : 10
            );
            return paginatedDTOs;
        }

        public async Task<bool> InvisibleAsync(int id)
        {
            var feedback = await _feedbackRepository.GetByIdAsync(id);
            if (feedback == null)
            {
                return false;
            }

            feedback.IsVisible = false;
            var result = await _feedbackRepository.UpdateAsync(feedback);
            return result;
        }


        public async Task<PaginatedList<FeedbackCustomerDto>> ListCustomerPagingAsync(
            int pageNumber,
            int pageSize,
            int restaurantId,
            int? star)
        {
            var query = await _feedbackRepository.QueryableAsync();

            var feedbacks = query
                .Include(x => x.Customer)
                .Where(x => x.RestaurantId == restaurantId);

            if (star.HasValue)
            {
                feedbacks = feedbacks.Where(x => x.Star == star.Value);
            }

            var queryDTO = feedbacks
                .OrderByDescending(x => x.FeedbackId)
                .Select(r => _mapper.Map<FeedbackCustomerDto>(r));

            var paginatedDTOs = await PaginatedList<FeedbackCustomerDto>.CreateAsync(
                queryDTO.AsNoTracking(),
                pageNumber > 0 ? pageNumber : 1,
                pageSize > 0 ? pageSize : 10
            );

            return paginatedDTOs;
        }


        public async Task<PaginatedList<FeedbackAdminDto>> ListAdminPagingAsync(
            int pageNumber,
            int pageSize,
            int? star,
            string? content)
        {
            var query = await _feedbackRepository.QueryableAsync();

            var feedbacks = query.AsQueryable();

            if (star.HasValue)
            {
                feedbacks = feedbacks.Where(x => x.Star == star.Value);
            }

            if (!string.IsNullOrWhiteSpace(content))
            {
                feedbacks = feedbacks.Where(x => x.Content != null && x.Content.Contains(content));
            }

            var queryDTO = feedbacks
                .Include(x => x.Customer)
                .Include(x => x.Restaurant)
                .OrderByDescending(x => x.FeedbackId)
                .Select(r => _mapper.Map<FeedbackAdminDto>(r));

            var paginatedDTOs = await PaginatedList<FeedbackAdminDto>.CreateAsync(
                queryDTO.AsNoTracking(),
                pageNumber > 0 ? pageNumber : 1,
                pageSize > 0 ? pageSize : 10
            );

            return paginatedDTOs;
        }

        public async Task<PaginatedList<FeedbackRestaurantDto>> ListRestaurantPagingAsync(
            int pageNumber,
            int pageSize,
            int restaurantId,
            int? star,
            string? content)
        {
            var query = await _feedbackRepository.QueryableAsync();

            var feedbacks = query.Where(x => x.RestaurantId == restaurantId);

            if (star.HasValue)
            {
                feedbacks = feedbacks.Where(x => x.Star == star.Value);
            }

            if (!string.IsNullOrWhiteSpace(content))
            {
                feedbacks = feedbacks.Where(x => x.Content != null && x.Content.Contains(content));
            }

            var queryDTO = feedbacks.Include(x => x.Customer)
                .OrderByDescending(x => x.FeedbackId)
                .Select(r => _mapper.Map<FeedbackRestaurantDto>(r));

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
