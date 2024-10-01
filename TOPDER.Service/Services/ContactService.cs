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

        public async Task<bool> AddAsync(ContactDto contactDto)
        {
            contactDto.ContactId = 0;
            var contact = _mapper.Map<Contact>(contactDto);
            return await _contactRepository.CreateAsync(contact);
        }

        public async Task<PaginatedList<ContactDto>> GetPagingAsync(int pageNumber, int pageSize)
        {
            var query = await _contactRepository.QueryableAsync();

            var contacts = query.OrderByDescending(x => x.ContactId);

            var queryDTO = contacts.Select(r => _mapper.Map<ContactDto>(r));

            var paginatedDTOs = await PaginatedList<ContactDto>.CreateAsync(
                queryDTO.AsNoTracking(),
                pageNumber > 0 ? pageNumber : 1,
                pageSize > 0 ? pageSize : 10
            );
            return paginatedDTOs;
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
        public async Task<PaginatedList<ContactDto>> SearchPagingAsync(int pageNumber, int pageSize, string contactContent, string topicContent)
        {
            var query = await _contactRepository.QueryableAsync();

            var filteredContacts = query.Where(x =>
                (string.IsNullOrEmpty(contactContent) || x.Content.Contains(contactContent)) &&
                (string.IsNullOrEmpty(topicContent) || x.Topic.Contains(topicContent))
            );

            var queryDTO = filteredContacts.Select(r => _mapper.Map<ContactDto>(r));

            var paginatedDTOs = await PaginatedList<ContactDto>.CreateAsync(
                queryDTO.AsNoTracking(),
                pageNumber > 0 ? pageNumber : 1,
                pageSize > 0 ? pageSize : 10
            );

            return paginatedDTOs;
        }

    }
}
