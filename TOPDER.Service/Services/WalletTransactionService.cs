using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Repository.Entities;
using TOPDER.Repository.IRepositories;
using TOPDER.Repository.Repositories;
using TOPDER.Service.Dtos.Discount;
using TOPDER.Service.Dtos.Notification;
using TOPDER.Service.Dtos.Wallet;
using TOPDER.Service.Dtos.WalletTransaction;
using TOPDER.Service.IServices;
using TOPDER.Service.Utils;
using static TOPDER.Service.Common.ServiceDefinitions.Constants;

namespace TOPDER.Service.Services
{
    public class WalletTransactionService : IWalletTransactionService
    {
        private readonly IMapper _mapper;
        private readonly IWalletTransactionRepository _walletTransactionRepository;
        private readonly INotificationService _notificationService;


        public WalletTransactionService(IWalletTransactionRepository walletTransactionRepository, IMapper mapper, INotificationService notificationService)
        {
            _walletTransactionRepository = walletTransactionRepository;
            _mapper = mapper;
            _notificationService = notificationService;
        }
        public async Task<bool> AddAsync(WalletTransactionDto walletTransactionDto)
        {   
            var walletTransaction = _mapper.Map<WalletTransaction>(walletTransactionDto);
            return await _walletTransactionRepository.CreateAsync(walletTransaction);
        }

        public async Task<WalletTransaction> AddRechargeAsync(WalletTransactionDto walletTransactionDto)
        {
            var walletTransaction = _mapper.Map<WalletTransaction>(walletTransactionDto);
            return await _walletTransactionRepository.CreateAndReturnAsync(walletTransaction);
        }


        public async Task<PaginatedList<WalletTransactionAdminDto>> GetAdminPagingAsync(int pageNumber, int pageSize, string? status)
        {
            var queryable = await _walletTransactionRepository.QueryableAsync();

            if (!string.IsNullOrEmpty(status))
            {
                queryable = queryable
                    .Include(x => x.Wallet)
                    .Where(x => x.Status.Equals(status));
            }

            var queryDTO = queryable.Select(r => _mapper.Map<WalletTransactionAdminDto>(r));

            var paginatedDTOs = await PaginatedList<WalletTransactionAdminDto>.CreateAsync(
                queryDTO.AsNoTracking(),
                pageNumber > 0 ? pageNumber : 1,
                pageSize > 0 ? pageSize : 10
            );
            return paginatedDTOs;
        }

        public async Task<PaginatedList<WalletTransactionDto>> GetPagingAsync(int pageNumber, int pageSize, int uid, string? status)
        {
            var queryable = await _walletTransactionRepository.QueryableAsync();

            var query = queryable.Include(x => x.Wallet)
                .Where(x => x.Wallet.Uid == uid);

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(x => x.Status.Equals(status));
            }

            var paginatedDTOs = await PaginatedList<WalletTransactionDto>.CreateAsync(
                query.Select(r => _mapper.Map<WalletTransactionDto>(r)).AsNoTracking(),
                pageNumber > 0 ? pageNumber : 1,
                pageSize > 0 ? pageSize : 10
            );

            return paginatedDTOs;
        }


        public async Task<WalletBalanceDto> GetWalletBalanceAsync(int transactionId)
        {
            var queryable = await _walletTransactionRepository.QueryableAsync();

            var queryDTO = await queryable
                .Include(x => x.Wallet)
                .ThenInclude(x => x.UidNavigation)
                .FirstOrDefaultAsync(x => x.TransactionId == transactionId);

            if (queryDTO == null)
            {
                throw new KeyNotFoundException("Transaction not found.");
            }

            var totalBalance = queryDTO.Wallet.WalletBalance + queryDTO.TransactionAmount;

            WalletBalanceDto walletBalanceDto = new WalletBalanceDto()
            {
                Uid = queryDTO.Wallet.UidNavigation.Uid, 
                WalletId = queryDTO.WalletId,
                WalletBalance = totalBalance 
            };

            return walletBalanceDto;
        }



        public async Task<bool> UpdateStatus(int TransactionId, string status)
        {
            var walletTransaction = await _walletTransactionRepository.GetByIdAsync(TransactionId);

            if (walletTransaction == null)
            {
                return false;
            }
            if (walletTransaction.Status.Equals(status))
            {
                return true;
            }

            walletTransaction.Status = status;
            return await _walletTransactionRepository.UpdateAsync(walletTransaction);
        }

    }
}
