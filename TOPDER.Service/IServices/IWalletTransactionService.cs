using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Repository.Entities;
using TOPDER.Service.Dtos.RestaurantTable;
using TOPDER.Service.Dtos.Wallet;
using TOPDER.Service.Dtos.WalletTransaction;
using TOPDER.Service.Utils;

namespace TOPDER.Service.IServices
{
    public interface IWalletTransactionService
    {
        Task<bool> AddAsync(WalletTransactionDto walletTransactionDto);
        Task<WalletTransaction> AddRechargeAsync(WalletTransactionDto walletTransactionDto);
        Task<bool> UpdateStatus(int TransactionId, string status);
        Task<WalletBalanceDto> GetWalletBalanceAsync(int transactionId);
        Task<List<WalletTransactionAdminDto>> GetWalletTransactionWithDrawAsync(string? status);
        Task<List<WalletTransactionDto>> GetWalletTransactionHistoryAsync(int uid);

    }
}
