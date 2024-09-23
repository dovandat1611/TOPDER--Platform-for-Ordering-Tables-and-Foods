using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Repository.Entities;
using TOPDER.Repository.IRepositories;
using TOPDER.Repository.Repositories;
using TOPDER.Service.Dtos.CategoryRestaurant;
using TOPDER.Service.Dtos.Chat;
using TOPDER.Service.IServices;
using TOPDER.Service.Utils;

namespace TOPDER.Service.Services
{
    public class ChatService : IChatService
    {
        private readonly IMapper _mapper;
        private readonly IChatRepository _chatRepository;

        public ChatService(IChatRepository chatRepository, IMapper mapper)
        {
            _chatRepository = chatRepository;
            _mapper = mapper;
        }
        public async Task<bool> AddAsync(CreateorUpdateChatDto createChatDto)
        {
            var chat = _mapper.Map<Chat>(createChatDto);
            return await _chatRepository.CreateAsync(chat);
        }

        public async Task<CreateorUpdateChatDto> GetItemAsync(int id)
        {
            var query = await _chatRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException($"Chat với id {id} không tồn tại.");
            var createorUpdateChatDto = _mapper.Map<CreateorUpdateChatDto>(query);
            return createorUpdateChatDto;
        }

        public async Task<List<ChatDto>> GetListAsync(int chatBoxId)
        {
            var query = await _chatRepository.QueryableAsync();

            var queryDTO = await query
                .Where(x => x.ChatBoxId == chatBoxId)  
                .ProjectTo<ChatDto>(_mapper.ConfigurationProvider)  
                .ToListAsync();  

            return queryDTO;
        }


        public async Task<bool> RemoveAsync(int id)
        {
            var chat = await _chatRepository.GetByIdAsync(id);
            if (chat == null)
            {
                return false;
            }
            var result = await _chatRepository.DeleteAsync(id);
            return result;
        }

        public async Task<bool> UpdateAsync(CreateorUpdateChatDto createChatDto)
        {
            var existingChat = await _chatRepository.GetByIdAsync(createChatDto.ChatId);
            if (existingChat == null)
            {
                return false;
            }
            var chat = _mapper.Map<Chat>(createChatDto);
            return await _chatRepository.UpdateAsync(chat);
        }
    }
}
