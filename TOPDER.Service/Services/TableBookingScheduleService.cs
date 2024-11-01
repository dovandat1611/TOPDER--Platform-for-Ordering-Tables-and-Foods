using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Repository.Entities;
using TOPDER.Repository.IRepositories;
using TOPDER.Repository.Repositories;
using TOPDER.Service.Dtos.Blog;
using TOPDER.Service.Dtos.CategoryRestaurant;
using TOPDER.Service.Dtos.TableBookingSchedule;
using TOPDER.Service.IServices;

namespace TOPDER.Service.Services
{
    public class TableBookingScheduleService : ITableBookingScheduleService
    {
        private readonly IMapper _mapper;
        private readonly ITableBookingScheduleRepository _tableBookingScheduleRepository;

        public TableBookingScheduleService(ITableBookingScheduleRepository tableBookingScheduleRepository, IMapper mapper)
        {
            _tableBookingScheduleRepository = tableBookingScheduleRepository;
            _mapper = mapper;
        }
        public async Task<bool> AddAsync(CreateTableBookingScheduleDto restaurantTableDto)
        {
            List<TableBookingSchedule> tableBookingSchedules = new List<TableBookingSchedule>();
            foreach(var taleId in restaurantTableDto.TableIds)
            {
                TableBookingSchedule tableBookingSchedule = new TableBookingSchedule()
                {
                    ScheduleId = 0,
                    TableId = taleId,
                    RestaurantId = restaurantTableDto.RestaurantId,
                    StartTime = restaurantTableDto.StartTime,
                    EndTime = restaurantTableDto.EndTime,
                    Notes = restaurantTableDto.Notes
                };
                tableBookingSchedules.Add(tableBookingSchedule);
            }

            return await _tableBookingScheduleRepository.CreateRangeAsync(tableBookingSchedules);
        }

        public async Task<TableBookingScheduleDto> GetItemAsync(int id)
        {
            var query = await _tableBookingScheduleRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Table Booking Schedule với id {id} không tồn tại.");
            var tableBookingSchedule = _mapper.Map<TableBookingScheduleDto>(query);
            return tableBookingSchedule;
        }

        public async Task<List<TableBookingScheduleViewDto>> GetTableBookingScheduleListAsync(int restaurantId)
        {
            var queryable = await _tableBookingScheduleRepository.QueryableAsync();

            var query = await queryable
                .Include(x => x.Restaurant)
                .Include(x => x.Table)
                .Where(x => x.RestaurantId == restaurantId)
                .ToListAsync();  

            var tableBookingSchedules = _mapper.Map<List<TableBookingScheduleViewDto>>(query);

            return tableBookingSchedules;
        }


        public async Task<bool> RemoveAsync(int id)
        {
            var tableBookingSchedule = await _tableBookingScheduleRepository.GetByIdAsync(id);
            if (tableBookingSchedule == null)
            {
                return false;
            }
            var result = await _tableBookingScheduleRepository.DeleteAsync(id);
            return result;
        }

        public async Task<bool> UpdateAsync(TableBookingScheduleDto tableBookingSchedule)
        {
            var existingTableBookingSchedule = await _tableBookingScheduleRepository.GetByIdAsync(tableBookingSchedule.ScheduleId);
            if (existingTableBookingSchedule == null)
            {
                return false;
            }
            existingTableBookingSchedule.StartTime = tableBookingSchedule.StartTime;
            existingTableBookingSchedule.EndTime = tableBookingSchedule.EndTime;
            existingTableBookingSchedule.TableId = tableBookingSchedule.TableId;
            existingTableBookingSchedule.Notes = tableBookingSchedule.Notes;
            return await _tableBookingScheduleRepository.UpdateAsync(existingTableBookingSchedule);
        }


    }
}
