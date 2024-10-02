using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Service.Dtos.Discount;
using TOPDER.Service.Dtos.ExternalLogin;
using TOPDER.Service.Dtos.Role;
using TOPDER.Service.Dtos.Wallet;

namespace TOPDER.Service.IServices
{
    public interface IWalletService
    {
        Task<bool> AddWalletBalanceAsync(WalletBalanceDto walletBalanceDto);
        Task<bool> UpdateWalletBalanceAsync(WalletBalanceDto walletBalanceDto);
        Task<bool> UpdateWalletBalanceOrderAsync(WalletBalanceOrderDto walletBalanceDto);
        Task<bool> AddWalletBankAsync(WalletBankDto walletBankDto);
        Task<bool> UpdateWalletBankAsync(WalletBankDto walletBankDto);
        Task<decimal> GetBalanceAsync(int walletId, int Uid);
        Task<decimal> GetBalanceOrderAsync(int Uid);
        Task<bool> AddOTPAsync(WalletOtpDto walletOtpDto);
        Task<bool> CheckExistWalletBalance(int Uid);
        Task<bool> CheckExistOTP(int Uid);
        Task<bool> CheckOTP(int Uid, string Otp);
    }
}
