using System;
using System.Collections.Generic;

namespace TOPDER.Repository.Entities
{
    public partial class Customer
    {
        public Customer()
        {
            ChatBoxes = new HashSet<ChatBox>();
            Feedbacks = new HashSet<Feedback>();
            Orders = new HashSet<Order>();
            Wishlists = new HashSet<Wishlist>();
        }

        public int Uid { get; set; }
        public string Name { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string? Image { get; set; }
        public DateTime? Dob { get; set; }
        public string? Gender { get; set; }

        public virtual User UidNavigation { get; set; } = null!;
        public virtual ICollection<ChatBox> ChatBoxes { get; set; }
        public virtual ICollection<Feedback> Feedbacks { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Wishlist> Wishlists { get; set; }
    }
}
