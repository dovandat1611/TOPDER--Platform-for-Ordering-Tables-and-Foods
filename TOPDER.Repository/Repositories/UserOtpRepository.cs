using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Repository.Entities;
using TOPDER.Repository.IRepositories;

namespace TOPDER.Repository.Repositories
{
    public class UserOtpRepository : GenericRepository<UserOtp>, IUserOtpRepository
    {
        private readonly TopderDBContext _dbContext;

        public UserOtpRepository(TopderDBContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<UserOtp?> GetValidOtpAsync(int userId, string otp)
        {
            return await _dbContext.UserOtps
                .Where(o => o.Uid == userId && o.OtpCode == otp && o.IsUse == false && o.ExpiresAt > DateTime.Now)
                .FirstOrDefaultAsync();
        }
    }
}
