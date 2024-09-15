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
            Notifications = new HashSet<Notification>();
            ReportReportedByNavigations = new HashSet<Report>();
            ReportReportedOnNavigations = new HashSet<Report>();
        }

        public int Uid { get; set; }
        public int RoleId { get; set; }
        public string Email { get; set; } = null!;
        public string? Password { get; set; }
        public string? OtpCode { get; set; }
        public bool IsVerify { get; set; }
        public int Status { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool IsExternalLogin { get; set; }

        public virtual Role Role { get; set; } = null!;
        public virtual Admin? Admin { get; set; }
        public virtual Customer? Customer { get; set; }
        public virtual Restaurant? Restaurant { get; set; }
        public virtual ICollection<Chat> Chats { get; set; }
        public virtual ICollection<Contact> Contacts { get; set; }
        public virtual ICollection<ExternalLogin> ExternalLogins { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
        public virtual ICollection<Report> ReportReportedByNavigations { get; set; }
        public virtual ICollection<Report> ReportReportedOnNavigations { get; set; }
    }
}
