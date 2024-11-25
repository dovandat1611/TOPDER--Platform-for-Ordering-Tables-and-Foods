using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Repository.Entities;
using TOPDER.Repository.IRepositories;
using TOPDER.Repository.Repositories;
using TOPDER.Service.Dtos.Feedback;
using TOPDER.Service.Dtos.FeedbackReply;
using TOPDER.Service.IServices;

namespace TOPDER.Service.Services
{
    public class FeedbackReplyService : IFeedbackReplyService
    {
        private readonly IMapper _mapper;
        private readonly IFeedbackReplyRepository _feedbackReplyRepository;
        private readonly IFeedbackRepository _feedbackRepository;

        public FeedbackReplyService(IFeedbackReplyRepository feedbackReplyRepository, IMapper mapper, IFeedbackRepository feedbackRepository)
        {
            _feedbackReplyRepository = feedbackReplyRepository;
            _feedbackRepository = feedbackRepository;
            _mapper = mapper;
        }

        public async Task<FeedbackDto> AddAsync(CreateFeedbackReplyDto feedbackReplyDto)
        {
            FeedbackReply feedbackReply = new FeedbackReply()
            {
                ReplyId = 0,
                FeedbackId = feedbackReplyDto.FeedbackId,
                RestaurantId = feedbackReplyDto.RestaurantId,
                Content = feedbackReplyDto.Content,
                CreateDate = DateTime.Now,
                IsVisible = true,
            };

            var checkCreate = await _feedbackReplyRepository.CreateAsync(feedbackReply);

            if(checkCreate == true)
            {
                var feedback = await _feedbackRepository.GetByIdAsync(feedbackReplyDto.FeedbackId);

                if(feedback != null)
                {
                    return _mapper.Map<FeedbackDto>(feedback);
                }

            }
            return null;
        }

        public async Task<bool> InvisibleAsync(int id)
        {
            var feedbackReply = await _feedbackReplyRepository.GetByIdAsync(id);
            if (feedbackReply == null)
            {
                return false;
            }

            feedbackReply.IsVisible = false;
            var result = await _feedbackReplyRepository.UpdateAsync(feedbackReply);
            return result;
        }

    }
}
