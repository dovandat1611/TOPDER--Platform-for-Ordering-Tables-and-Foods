using System;
using System.Collections.Generic;

namespace TOPDER.Repository.Entities
{
    public partial class Admin
    {
        public Admin()
        {
            AdvertisementPricings = new HashSet<AdvertisementPricing>();
            Blogs = new HashSet<Blog>();
            PolicySystems = new HashSet<PolicySystem>();
        }

        public int Uid { get; set; }
        public string Name { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public DateTime? Dob { get; set; }
        public string? Image { get; set; }

        public virtual User UidNavigation { get; set; } = null!;
        public virtual ICollection<AdvertisementPricing> AdvertisementPricings { get; set; }
        public virtual ICollection<Blog> Blogs { get; set; }
        public virtual ICollection<PolicySystem> PolicySystems { get; set; }
    }
}
