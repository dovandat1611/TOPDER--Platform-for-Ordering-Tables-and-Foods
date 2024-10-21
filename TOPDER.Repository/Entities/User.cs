using System;
using System.Collections.Generic;

namespace TOPDER.Repository.Entities
{
    public partial class User
    {
        public User()
        {
            Chats = new HashSet<Chat>();
            Contacts = new HashSet<Contact>();
            ExternalLogins = new HashSet<ExternalLogin>();
            Logs = new HashSet<Log>();
            Notifications = new HashSet<Notification>();
            ReportReportedByNavigations = new HashSet<Report>();
            ReportReportedOnNavigations = new HashSet<Report>();
            UserOtps = new HashSet<UserOtp>();
            Wallets = new HashSet<Wallet>();
        }

        public int Uid { get; set; }
        public int RoleId { get; set; }
        public string Email { get; set; } = null!;
        public string? Password { get; set; }
        public bool IsVerify { get; set; }
        public string Status { get; set; } = null!;
        public DateTime? CreatedAt { get; set; }
        public bool IsExternalLogin { get; set; }

        public virtual Role Role { get; set; } = null!;
        public virtual Admin? Admin { get; set; }
        public virtual Customer? Customer { get; set; }
        public virtual Restaurant? Restaurant { get; set; }
        public virtual ICollection<Chat> Chats { get; set; }
        public virtual ICollection<Contact> Contacts { get; set; }
        public virtual ICollection<ExternalLogin> ExternalLogins { get; set; }
        public virtual ICollection<Log> Logs { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
        public virtual ICollection<Report> ReportReportedByNavigations { get; set; }
        public virtual ICollection<Report> ReportReportedOnNavigations { get; set; }
        public virtual ICollection<UserOtp> UserOtps { get; set; }
        public virtual ICollection<Wallet> Wallets { get; set; }
    }
}
