﻿using AutoMapper;
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

            var filteredQuery = queryable
                .Include(x => x.Restaurant)
                .Include(x => x.Customer)
                .Where(x => x.CustomerId == userId || x.RestaurantId == userId);

            var queryDTO = _mapper.Map<List<ChatBoxDto>>(filteredQuery);

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
    }
}
