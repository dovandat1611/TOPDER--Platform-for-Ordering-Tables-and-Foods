﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Repository.Entities;
using TOPDER.Repository.IRepositories;
using TOPDER.Repository.Repositories;
using TOPDER.Service.Dtos.Discount;
using TOPDER.Service.Dtos.Wallet;
using TOPDER.Service.Dtos.WalletTransaction;
using TOPDER.Service.IServices;
using TOPDER.Service.Utils;

namespace TOPDER.Service.Services
{
    public class WalletTransactionService : IWalletTransactionService
    {
        private readonly IMapper _mapper;
        private readonly IWalletTransactionRepository _walletTransactionRepository;

        public WalletTransactionService(IWalletTransactionRepository walletTransactionRepository, IMapper mapper)
        {
            _walletTransactionRepository = walletTransactionRepository;
            _mapper = mapper;
        }
        public async Task<bool> AddAsync(WalletTransactionDto walletTransactionDto)
        {
            var walletTransaction = _mapper.Map<WalletTransaction>(walletTransactionDto);
            return await _walletTransactionRepository.CreateAsync(walletTransaction);
        }

        public async Task<PaginatedList<WalletTransactionAdminDto>> GetPagingAsync(int pageNumber, int pageSize, string status)
        {
            var queryable = await _walletTransactionRepository.QueryableAsync();

            if (!string.IsNullOrEmpty(status))
            {
                queryable = queryable.Where(x => x.Status.Equals(status));
            }

            var queryDTO = queryable.Select(r => _mapper.Map<WalletTransactionAdminDto>(r));

            var paginatedDTOs = await PaginatedList<WalletTransactionAdminDto>.CreateAsync(
                queryDTO.AsNoTracking(),
                pageNumber > 0 ? pageNumber : 1,
                pageSize > 0 ? pageSize : 10
            );
            return paginatedDTOs;
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