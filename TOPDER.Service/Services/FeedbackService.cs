using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Repository.IRepositories;
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

        public Task<bool> AddAsync(FeedbackDto feedbackDto)
        {
            throw new NotImplementedException();
        }

        public Task<PaginatedList<FeedbackAdminDto>> GetAdminPagingAsync(int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }

        public Task<PaginatedList<FeedbackCustomerDto>> GetCustomerPagingAsync(int pageNumber, int pageSize, int restaurantId)
        {
            throw new NotImplementedException();
        }

        public Task<PaginatedList<FeedbackHistoryDto>> GetHistoryCustomerPagingAsync(int pageNumber, int pageSize, int customerId)
        {
            throw new NotImplementedException();
        }


        public Task<PaginatedList<FeedbackRestaurantDto>> GetRestaurantPagingAsync(int pageNumber, int pageSize, int restaurantId)
        {
            throw new NotImplementedException();
        }

        public Task<PaginatedList<FeedbackCustomerDto>> GetSearchCustomerPagingAsync(int pageNumber, int pageSize, int restaurantId, int star)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<PaginatedList<FeedbackAdminDto>> SearchAdminPagingAsync(int pageNumber, int pageSize, int star)
        {
            throw new NotImplementedException();
        }

        public Task<PaginatedList<FeedbackRestaurantDto>> SearchRestaurantPagingAsync(int pageNumber, int pageSize, int restaurantId, int star)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(FeedbackDto feedbackDto)
        {
            throw new NotImplementedException();
        }
    }
}
