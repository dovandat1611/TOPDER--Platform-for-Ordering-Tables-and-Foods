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

        public async Task<List<FeedbackHistoryDto>> GetHistoryCustomerPagingAsync(int customerId)
        {
            var query = await _feedbackRepository.QueryableAsync();
            var feedbackReplyQueryable = await _feedbackReplyRepository.QueryableAsync();

            var feedbacks = await query
                .Include(x => x.Restaurant)
                .Where(x => x.CustomerId == customerId && x.IsVisible == true)
                .OrderByDescending(x => x.FeedbackId).ToListAsync();

            var queryDTO = _mapper.Map<List<FeedbackHistoryDto>>(feedbacks);

            foreach (var feedback in queryDTO)
            {
                var feedbackReply = await feedbackReplyQueryable
                    .Include(x => x.Restaurant)
                    .FirstOrDefaultAsync(x => x.FeedbackId == feedback.FeedbackId && x.IsVisible == true);

                if (feedbackReply != null)
                {
                    feedback.FeedbackReplyCustomer = _mapper.Map<FeedbackReplyCustomerDto>(feedbackReply);
                    feedback.isReply = true;
                }
                else
                {
                    feedback.isReply = false;
                }
            }

            return queryDTO;
        }


        public async Task<FeedbackDto> InvisibleAsync(int id)
        {
            var feedback = await _feedbackRepository.GetByIdAsync(id);
            if (feedback == null)
            {
                return null;
            }

            feedback.IsVisible = false;
            var result = await _feedbackRepository.UpdateAsync(feedback);
            if(result == true)
            {
                var feedbackDto = _mapper.Map<FeedbackDto>(feedback);
                return feedbackDto;
            }
            return null;
        }


        public async Task<List<FeedbackCustomerDto>> ListCustomerPagingAsync(int restaurantId)
        {
            var query = await _feedbackRepository.QueryableAsync();
            var feedbackReplyQueryable = await _feedbackReplyRepository.QueryableAsync();

            var feedbacks = await query
                .Include(x => x.Customer)
                .Where(x => x.RestaurantId == restaurantId && x.IsVisible == true)
                .OrderByDescending(x => x.FeedbackId).ToListAsync();

            var queryDTO = _mapper.Map<List<FeedbackCustomerDto>>(feedbacks);

            foreach( var feedback in queryDTO)
            {
                var feedbackReply = await feedbackReplyQueryable.Include(x => x.Restaurant).FirstOrDefaultAsync(x => x.FeedbackId == feedback.FeedbackId && x.IsVisible == true);
                if (feedbackReply != null)
                {
                    feedback.FeedbackReplyCustomer = _mapper.Map<FeedbackReplyCustomerDto>(feedbackReply);
                    feedback.isReply = true;
                }
                else
                {
                    feedback.isReply = false;
                }
            }

            return queryDTO;
        }

        public async Task<List<FeedbackAdminDto>> ListAdminPagingAsync()
        {
            var query = await _feedbackRepository.QueryableAsync();
            var feedbackReplyQueryable = await _feedbackReplyRepository.QueryableAsync();

            var feedbacks = await query.Include(x => x.Customer)
                .Include(x => x.Restaurant)
                .Where(x => x.IsVisible == true).OrderByDescending(x => x.FeedbackId).ToListAsync();

            var queryDTO = _mapper.Map<List<FeedbackAdminDto>>(feedbacks);

            foreach (var feedback in queryDTO)
            {
                var feedbackReply = await feedbackReplyQueryable.Include(x => x.Restaurant).FirstOrDefaultAsync(x => x.FeedbackId == feedback.FeedbackId && x.IsVisible == true);
                if (feedbackReply != null)
                {
                    feedback.FeedbackReplyCustomer = _mapper.Map<FeedbackReplyCustomerDto>(feedbackReply);
                    feedback.isReply = true;
                }
                else
                {
                    feedback.isReply = false;
                }
            }
            return queryDTO;
        }

        public async Task<List<FeedbackRestaurantDto>> ListRestaurantPagingAsync(int restaurantId)
        {
            var query = await _feedbackRepository.QueryableAsync();
            var feedbackReplyQueryable = await _feedbackReplyRepository.QueryableAsync();

            var feedbacks = await query.Include(x => x.Customer).Where(x => x.RestaurantId == restaurantId && x.IsVisible == true).OrderByDescending(x => x.FeedbackId).ToListAsync();

            var queryDTO = _mapper.Map<List<FeedbackRestaurantDto>>(feedbacks);

            foreach (var feedback in queryDTO)
            {
                var feedbackReply = await feedbackReplyQueryable
                    .Include(x => x.Restaurant)
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
            return queryDTO;
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
