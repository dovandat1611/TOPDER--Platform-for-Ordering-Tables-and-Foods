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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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

        public async Task<List<ChatBoxDto>> GetChatListAsync(int userId)
        {
            var queryable = await _chatBoxRepository.QueryableAsync();

            var filteredQuery = await queryable
                .Include(x => x.Restaurant)
                .Include(x => x.Customer)
                .Where(x => x.CustomerId == userId || x.RestaurantId == userId).ToListAsync();

            var queryDTO = _mapper.Map<List<ChatBoxDto>>(filteredQuery);

            if (queryDTO.Count() > 0)
            {
                foreach (var chatBox in filteredQuery)
                {
                    var dto = queryDTO.FirstOrDefault(x => x.ChatBoxId == chatBox.ChatBoxId);
                    if (dto != null)
                    {
                        dto.IsRead = chatBox.CustomerId == userId
                            ? chatBox.IsCustomerRead
                            : chatBox.IsRestaurantRead;
                    }
                }
            }

            return queryDTO;
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

        public async Task<bool> CheckExistAsync(int customerId, int restaurantId)
        {
            var queryable = await _chatBoxRepository.QueryableAsync();
            var checkExist = await queryable.FirstOrDefaultAsync(x => x.CustomerId == customerId && x.RestaurantId == restaurantId);
            if(checkExist == null)
            {
                return false;
            }
            return true;
        }

        public async Task<bool> IsReadAsync(int uid, int chatboxId)
        {
            var queryable = await _chatBoxRepository.QueryableAsync();
            var chatBox = await queryable.FirstOrDefaultAsync(x => (x.CustomerId == uid || x.RestaurantId == uid) && x.ChatBoxId == chatboxId);
            if(chatBox == null)
            {
                return false;
            }
            if (chatBox.CustomerId == uid)
            {
                chatBox.IsCustomerRead = true;
            }
            if (chatBox.RestaurantId == uid)
            {
                chatBox.IsRestaurantRead = true;
            }
            return await _chatBoxRepository.UpdateAsync(chatBox);
        }

        public async Task<bool> IsReadAllAsync(int uid)
        {
            var queryable = await _chatBoxRepository.QueryableAsync();
            var chatBoxList = await queryable.Where(x => (x.CustomerId == uid || x.RestaurantId == uid)).ToListAsync();
            if(chatBoxList == null)
            {
                return false;
            }
            foreach(var item in chatBoxList)
            {
                if (item.CustomerId == uid)
                {
                    item.IsCustomerRead = true;
                }
                if (item.RestaurantId == uid)
                {
                    item.IsRestaurantRead = true;
                }
                await _chatBoxRepository.UpdateAsync(item);
            }
            return true;
        }
    }
}
