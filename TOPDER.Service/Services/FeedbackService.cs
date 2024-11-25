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
using TOPDER.Service.Dtos.FeedbackReply;
using TOPDER.Service.IServices;
using TOPDER.Service.Utils;
using static TOPDER.Service.Common.ServiceDefinitions.Constants;

namespace TOPDER.Service.Services
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IMapper _mapper;
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly IFeedbackReplyRepository _feedbackReplyRepository;

        public FeedbackService(IFeedbackRepository feedbackRepository,
            IMapper mapper, IFeedbackReplyRepository feedbackReplyRepository) { 
            _feedbackRepository = feedbackRepository;
            _mapper = mapper;
            _feedbackReplyRepository = feedbackReplyRepository;
        }

        public async Task<FeedbackDto> AddAsync(FeedbackDto feedbackDto)
        {
            feedbackDto.FeedbackId = 0;
            var feedback = _mapper.Map<Feedback>(feedbackDto);
            feedback.CreateDate = DateTime.Now;
            feedback.Status = Common_Status.ACTIVE;
            var createFeedback = await _feedbackRepository.CreateAndReturnAsync(feedback);
            if (createFeedback != null)
            {
                return _mapper.Map<FeedbackDto>(createFeedback);
            }
            return null;
        }

        public async Task<PaginatedList<FeedbackHistoryDto>> GetHistoryCustomerPagingAsync(int pageNumber, int pageSize, int customerId)
        {
            var query = await _feedbackRepository.QueryableAsync();
            var feedbackReplyQueryable = await _feedbackReplyRepository.QueryableAsync();

            var feedbacks = query
                .Include(x => x.Restaurant)
                .OrderByDescending(x => x.FeedbackId)
                .Where(x => x.CustomerId == customerId && x.IsVisible == true);

            var queryDTO = feedbacks.Select(r => _mapper.Map<FeedbackHistoryDto>(r));

            foreach (var feedback in queryDTO)
            {
                var feedbackReply = await feedbackReplyQueryable
                    .Include(x => x.Restaurant)
                    .FirstOrDefaultAsync(x => x.FeedbackId == feedback.FeedbackId && x.IsVisible == true);

                if (feedbackReply != null)
                {
                    feedback.FeedbackReplyCustomer = _mapper.Map<FeedbackReplyCustomerDto>(feedbackReply);
                }
            }

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
            var feedbackReplyQueryable = await _feedbackReplyRepository.QueryableAsync();

            var feedbacks = query
                .Include(x => x.Customer)
                .Where(x => x.RestaurantId == restaurantId && x.IsVisible == true);

            if (star.HasValue)
            {
                feedbacks = feedbacks.Where(x => x.Star == star.Value);
            }

            var queryDTO = feedbacks
                .OrderByDescending(x => x.FeedbackId)
                .Select(r => _mapper.Map<FeedbackCustomerDto>(r));

            foreach( var feedback in queryDTO)
            {
                var feedbackReply = await feedbackReplyQueryable.Include(x => x.Restaurant).FirstOrDefaultAsync(x => x.FeedbackId == feedback.FeedbackId && x.IsVisible == true);
                if (feedbackReply != null)
                {
                    feedback.FeedbackReplyCustomer = _mapper.Map<FeedbackReplyCustomerDto>(feedbackReply);
                }
            }

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
            var feedbackReplyQueryable = await _feedbackReplyRepository.QueryableAsync();

            var feedbacks = query.Include(x => x.Customer)
                .Include(x => x.Restaurant)
                .Where(x => x.IsVisible == true).AsQueryable();

            if (star.HasValue)
            {
                feedbacks = feedbacks.Where(x => x.Star == star.Value);
            }

            if (!string.IsNullOrWhiteSpace(content))
            {
                feedbacks = feedbacks.Where(x => x.Content != null && x.Content.Contains(content));
            }

            var queryDTO = feedbacks.OrderByDescending(x => x.FeedbackId).Select(r => _mapper.Map<FeedbackAdminDto>(r));

            foreach (var feedback in queryDTO)
            {
                var feedbackReply = await feedbackReplyQueryable.Include(x => x.Restaurant).FirstOrDefaultAsync(x => x.FeedbackId == feedback.FeedbackId && x.IsVisible == true);
                if (feedbackReply != null)
                {
                    feedback.FeedbackReplyCustomer = _mapper.Map<FeedbackReplyCustomerDto>(feedbackReply);
                }
            }

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
            var feedbackReplyQueryable = await _feedbackReplyRepository.QueryableAsync();

            var feedbacks = query.Include(x => x.Customer).Where(x => x.RestaurantId == restaurantId && x.IsVisible == true);

            if (star.HasValue)
            {
                feedbacks = feedbacks.Where(x => x.Star == star.Value);
            }

            if (!string.IsNullOrWhiteSpace(content))
            {
                feedbacks = feedbacks.Where(x => x.Content != null && x.Content.Contains(content));
            }

            var queryDTO = feedbacks
                .OrderByDescending(x => x.FeedbackId)
                .Select(r => _mapper.Map<FeedbackRestaurantDto>(r));


            foreach (var feedback in queryDTO)
            {
                var feedbackReply = await feedbackReplyQueryable
                    .FirstOrDefaultAsync(x => x.FeedbackId == feedback.FeedbackId && x.IsVisible == true);
                if (feedbackReply != null)
                {
                    feedback.FeedbackReply = _mapper.Map<FeedbackReplyDto>(feedbackReply);
                    feedback.isReply = true;
                }
                else
                {
                    feedback.isReply = false;
                }
            }

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

        public async Task<FeedbackDto> GetFeedbackAsync(int orderId)
        {
            var query = await _feedbackRepository.QueryableAsync();

            var feedback = await query.FirstOrDefaultAsync(x => x.OrderId == orderId);

            var feedbackDto = _mapper.Map<FeedbackDto>(feedback);

            return feedbackDto;
        }
    }
}
