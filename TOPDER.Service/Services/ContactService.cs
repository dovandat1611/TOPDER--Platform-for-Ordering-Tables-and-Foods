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
using TOPDER.Service.Dtos.Blog;
using TOPDER.Service.Dtos.Contact;
using TOPDER.Service.IServices;
using TOPDER.Service.Utils;
using static TOPDER.Service.Common.ServiceDefinitions.Constants;

namespace TOPDER.Service.Services
{
    public class ContactService : IContactService
    {
        private readonly IMapper _mapper;
        private readonly IContactRepository _contactRepository;

        public ContactService(IContactRepository contactRepository, IMapper mapper)
        {
            _contactRepository = contactRepository;
            _mapper = mapper;
        }

        public async Task<bool> AddAsync(CreateContactDto contactDto)
        {
            Contact contact = new Contact()
            {
                ContactId = 0,
                Uid = contactDto.Uid,
                Email = contactDto.Email,
                Name = contactDto.Name,
                Phone = contactDto.Phone,
                Topic = contactDto.Topic,
                Content = contactDto.Content,
                ContactDate = DateTime.Now,
                Status = Common_Status.ACTIVE
            };
            return await _contactRepository.CreateAsync(contact);
        }

        public async Task<bool> RemoveAsync(int id)
        {
            var contact = await _contactRepository.GetByIdAsync(id);
            if (contact == null)
            {
                return false;
            }
            var result = await _contactRepository.DeleteAsync(id);
            return result;
        }
        public async Task<PaginatedList<ContactDto>> SearchPagingAsync(int pageNumber, int pageSize, string? contactContent, string? topicContent)
        {
            // Lấy danh sách IQueryable từ repository
            var query = await _contactRepository.QueryableAsync();

            // Lọc dữ liệu dựa trên các tham số đầu vào
            var filteredContacts = query.Where(x =>
                (string.IsNullOrEmpty(contactContent) || x.Content.Contains(contactContent)) &&
                (string.IsNullOrEmpty(topicContent) || x.Topic.Contains(topicContent))
            ).OrderByDescending(x => x.ContactId).AsNoTracking(); // Sử dụng AsNoTracking trước khi Select()

            // Ánh xạ sang DTO
            var queryDTO = filteredContacts.Select(r => _mapper.Map<ContactDto>(r));

            // Tạo danh sách phân trang
            var paginatedDTOs = await PaginatedList<ContactDto>.CreateAsync(
                queryDTO,
                pageNumber > 0 ? pageNumber : 1,
                pageSize > 0 ? pageSize : 10
            );

            return paginatedDTOs;
        }


    }
}
