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
using TOPDER.Service.Dtos.Report;
using TOPDER.Service.IServices;
using TOPDER.Service.Utils;

namespace TOPDER.Service.Services
{
    public class ReportService : IReportService
    {
        private readonly IMapper _mapper;
        private readonly IReportRepository _reportRepository;

        public ReportService(IReportRepository reportRepository, IMapper mapper)
        {
            _reportRepository = reportRepository;
            _mapper = mapper;
        }
        public async Task<bool> AddAsync(ReportDto reportDto)
        {
            var report = _mapper.Map<Report>(reportDto);
            return await _reportRepository.CreateAsync(report);
        }

        public async Task<PaginatedList<ReportListDto>> GetPagingAsync(int pageNumber, int pageSize)
        {
            var query = await _reportRepository.QueryableAsync();

            var blogs = query
                .Include(x => x.ReportedByNavigation)
                .Include(x => x.ReportedOnNavigation)
                .OrderByDescending(x => x.ReportId);

            var queryDTO = blogs.Select(r => _mapper.Map<ReportListDto>(r));

            var paginatedDTOs = await PaginatedList<ReportListDto>.CreateAsync(
                queryDTO.AsNoTracking(),
                pageNumber > 0 ? pageNumber : 1,
                pageSize > 0 ? pageSize : 10
            );
            return paginatedDTOs;
        }

        public async Task<bool> RemoveAsync(int id)
        {
            var report = await _reportRepository.GetByIdAsync(id);
            if (report == null)
            {
                return false;
            }
            var result = await _reportRepository.DeleteAsync(id);
            return result;
        }

    }
}
