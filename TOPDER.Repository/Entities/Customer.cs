using System;
using System.Collections.Generic;

namespace TOPDER.Repository.Entities
{
    public partial class Customer
    {
        public Customer()
        {
            ChatBoxes = new HashSet<ChatBox>();
            Orders = new HashSet<Order>();
            Feedbacks = new HashSet<Feedback>();
            WalletTransactions = new HashSet<WalletTransaction>();
            Wishlists = new HashSet<Wishlist>();
        }

        public int Uid { get; set; }
        public string Name { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string? Image { get; set; }
        public DateTime? Dob { get; set; }
        public string? Gender { get; set; }
        public decimal WalletBalance { get; set; }

        public virtual User UidNavigation { get; set; } = null!;
        public virtual ICollection<ChatBox> ChatBoxes { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Feedback> Feedbacks { get; set; }
        public virtual ICollection<WalletTransaction> WalletTransactions { get; set; }
        public virtual ICollection<Wishlist> Wishlists { get; set; }
    }
}
