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
using TOPDER.Service.Dtos.Chat;
using TOPDER.Service.Dtos.ChatBox;
using TOPDER.Service.IServices;
using TOPDER.Service.Utils;

namespace TOPDER.Service.Services
{
    public class ChatBoxService : IChatBoxService
    {
        private readonly IMapper _mapper;
        private readonly IChatBoxRepository _chatBoxRepository;
        private readonly IChatRepository _chatRepository;


        public ChatBoxService(IChatBoxRepository chatBoxRepository, IMapper mapper, IChatRepository chatRepository)
        {
            _chatBoxRepository = chatBoxRepository;
            _mapper = mapper;
            _chatRepository = chatRepository;
        }
        public async Task<bool> AddAsync(CreateChatBoxDto chatBoxDto)
        {
            var chatBox = _mapper.Map<ChatBox>(chatBoxDto);
            return await _chatBoxRepository.CreateAsync(chatBox);
        }

        public async Task<ChatBoxDto> GetItemAsync(int id)
        {
            var query = await _chatBoxRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException($"Chat Box với id {id} không tồn tại.");
            var chatBoxDto = _mapper.Map<ChatBoxDto>(query);
            return chatBoxDto;
        }

        public async Task<PaginatedList<ChatBoxDto>> GetPagingAsync(int pageNumber, int pageSize, int userId)
        {
            var queryable = await _chatBoxRepository.QueryableAsync();

            var filteredQuery = queryable
                .Include(x => x.Restaurant)
                .Include(x => x.Customer)
                .Where(x => x.CustomerId == userId || x.RestaurantId == userId);


            var queryDTO = filteredQuery.Select(r => _mapper.Map<ChatBoxDto>(r));

            var paginatedDTOs = await PaginatedList<ChatBoxDto>.CreateAsync(
                queryDTO.AsNoTracking(),
                pageNumber > 0 ? pageNumber : 1,
                pageSize > 0 ? pageSize : 10
            );

            return paginatedDTOs;
        }

        public async Task<bool> RemoveAsync(int id)
        {
            var chatBox = await _chatBoxRepository.GetByIdAsync(id);
            if (chatBox == null)
            {
                return false; 
            }

            var queryAssociatedChats = await _chatRepository.QueryableAsync(); 

            var associatedChats = await queryAssociatedChats
                .Where(c => c.ChatBoxId == id) 
                .ToListAsync(); 

            foreach (var chat in associatedChats)
            {
                await _chatRepository.DeleteAsync(chat.ChatId); 
            }

            var result = await _chatBoxRepository.DeleteAsync(id);
            return result; 
        }


    }
}
