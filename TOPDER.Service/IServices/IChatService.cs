using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Service.Dtos.CategoryMenu;
using TOPDER.Service.Dtos.Chat;
using TOPDER.Service.Dtos.ChatBox;
using TOPDER.Service.Utils;

namespace TOPDER.Service.IServices
{
    public interface IChatService
    {
        Task<ChatDto> AddAsync(CreateChatDto createChatDto);
        Task<bool> UpdateAsync(UpdateChatDto updateChatDto);
        Task<bool> RemoveAsync(int id);
        Task<CreateChatDto> GetItemAsync(int id);
        Task<List<ChatDto>> GetListAsync(int chatBoxId);
    }
}
