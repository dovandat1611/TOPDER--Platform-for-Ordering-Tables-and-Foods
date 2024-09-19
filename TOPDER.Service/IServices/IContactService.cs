using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Service.Dtos.Blog;
using TOPDER.Service.Dtos.BlogGroup;
using TOPDER.Service.Dtos.Contact;
using TOPDER.Service.Utils;

namespace TOPDER.Service.IServices
{
    public interface IContactService
    {
        Task<bool> AddAsync(ContactDto contactDto);
        Task<bool> RemoveAsync(int id);
        Task<PaginatedList<ContactDto>> GetPagingAsync(int pageNumber, int pageSize);
        Task<PaginatedList<ContactDto>> SearchPagingAsync(int pageNumber, int pageSize, string contactContent);
    }
}
