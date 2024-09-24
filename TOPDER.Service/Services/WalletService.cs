using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Repository.Entities;
using TOPDER.Repository.IRepositories;
using TOPDER.Service.Dtos.Blog;
using TOPDER.Service.Dtos.Wallet;
using TOPDER.Service.IServices;

namespace TOPDER.Service.Services
{
    public class WalletService : IWalletService
    {
        private readonly IMapper _mapper;
        private readonly IWalletRepository _walletRepository;

        public WalletService(IWalletRepository walletRepository, IMapper mapper)
        {
            _walletRepository = walletRepository;
            _mapper = mapper;
        }

        public async Task<bool> AddOTPAsync(WalletOtpDto walletOtpDto)
        {
            var wallet = _mapper.Map<Wallet>(walletOtpDto);
            return await _walletRepository.UpdateAsync(wallet);
        }

        public async Task<bool> AddWalletBalanceAsync(WalletBalanceDto walletBalanceDto)
        {
            var wallet = _mapper.Map<Wallet>(walletBalanceDto);
            return await _walletRepository.CreateAsync(wallet);
        }

        public async Task<bool> AddWalletBankAsync(WalletBankDto walletBankDto)
        {
            var wallet = _mapper.Map<Wallet>(walletBankDto);
            return await _walletRepository.UpdateAsync(wallet);
        }

        public async Task<bool> CheckExistOTP(int Uid)
        {
            var query = await _walletRepository.QueryableAsync();

            var queryCheck = query.FirstOrDefault(x => x.Uid == Uid && x.OtpCode == null);

            return queryCheck == null;
        }


        public async Task<bool> CheckExistWalletBalance(int Uid)
        {
            var query = await _walletRepository.QueryableAsync();

            var queryCheck = query.FirstOrDefault(x => x.Uid == Uid && !string.IsNullOrEmpty(x.BankCode)
                                                      && !string.IsNullOrEmpty(x.AccountNo)
                                                      && !string.IsNullOrEmpty(x.AccountName));
            return queryCheck != null;
        }

        public async Task<bool> CheckOTP(int Uid, string Otp)
        {
            var query = await _walletRepository.QueryableAsync();

            var queryCheck = query.FirstOrDefault(x => x.Uid == Uid && x.OtpCode != null && x.OtpCode.Equals(Otp));

            if (queryCheck != null)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> UpdateWalletBalanceAsync(WalletBalanceDto walletBalanceDto)
        {
            var wallet = _mapper.Map<Wallet>(walletBalanceDto);
            return await _walletRepository.UpdateAsync(wallet);
        }

        public async Task<bool> UpdateWalletBankAsync(WalletBankDto walletBankDto)
        {
            var wallet = _mapper.Map<Wallet>(walletBankDto);
            return await _walletRepository.UpdateAsync(wallet);
        }


    }
}
