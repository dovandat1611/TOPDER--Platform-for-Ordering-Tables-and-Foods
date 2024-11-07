using System;
using System.Collections.Generic;

namespace TOPDER.Repository.Entities
{
    public partial class UserOtp
    {
        public int OtpId { get; set; }
        public int Uid { get; set; }
        public string? OtpCode { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool? IsUse { get; set; }
        public DateTime? CreatedAt { get; set; }

        public virtual User UidNavigation { get; set; } = null!;
    }
}
