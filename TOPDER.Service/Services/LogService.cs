using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Repository.IRepositories;
using TOPDER.Repository.Repositories;
using TOPDER.Service.Dtos.Log;
using TOPDER.Service.IServices;
using TOPDER.Service.Utils;
using TOPDER.Repository.Entities;
using Microsoft.EntityFrameworkCore;

namespace TOPDER.Service.Services
{
    public class LogService : ILogService
    {
        private readonly IMapper _mapper;
        private readonly ILogRepository _logRepository;

        public LogService(ILogRepository logRepository, IMapper mapper)
        {
            _logRepository = logRepository;
            _mapper = mapper;
        }
        public async Task<bool> AddAsync(LogDto logDto)
        {
            var log = _mapper.Map<Log>(logDto);
            return await _logRepository.CreateAsync(log);
        }

        public async Task<PaginatedList<LogDto>> GetPagingAsync(int pageNumber, int pageSize, int userId)
        {
            var queryable = await _logRepository.QueryableAsync();

            var query = queryable.Where(x => x.Uid == userId);

            var queryDTO = query.Select(r => _mapper.Map<LogDto>(r));

            var paginatedDTOs = await PaginatedList<LogDto>.CreateAsync(
                queryDTO.AsNoTracking(),
                pageNumber > 0 ? pageNumber : 1,
                pageSize > 0 ? pageSize : 10
            );
            return paginatedDTOs;
        }

    }
}
