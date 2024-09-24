using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Service.Dtos.RestaurantTable;
using TOPDER.Service.Dtos.Wallet;
using TOPDER.Service.Dtos.WalletTransaction;
using TOPDER.Service.Utils;

namespace TOPDER.Service.IServices
{
    public interface IWalletTransactionService
    {
        Task<bool> AddAsync(WalletTransactionDto walletTransactionDto);
        Task<bool> UpdateStatus(int TransactionId, string status);
        Task<PaginatedList<WalletTransactionAdminDto>> GetPagingAsync(int pageNumber, int pageSize, string status);

    }
}
