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
        Task<bool> AddAsync(CreateorUpdateChatDto createChatDto);
        Task<bool> UpdateAsync(CreateorUpdateChatDto createChatDto);
        Task<bool> RemoveAsync(int id);
        Task<CreateorUpdateChatDto> GetItemAsync(int id);
        Task<List<ChatDto>> GetListAsync(int chatBoxId);
    }
}
