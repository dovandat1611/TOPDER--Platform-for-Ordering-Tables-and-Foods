using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Service.Dtos.CategoryRestaurant;
using TOPDER.Service.Dtos.ChatBox;
using TOPDER.Service.Utils;

namespace TOPDER.Service.IServices
{
    public interface IChatBoxService
    {
        Task<bool> AddAsync(CreateChatBoxDto chatBoxDto);
        Task<bool> RemoveAsync(int id);
        Task<ChatBoxDto> GetItemAsync(int id);
        Task<List<ChatBoxDto>> GetChatListAsync(int userId);
    }
}
