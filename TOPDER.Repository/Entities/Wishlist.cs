using System;
using System.Collections.Generic;

namespace TOPDER.Repository.Entities
{
    public partial class Wishlist
    {
        public int WishlistId { get; set; }
        public int? CustomerId { get; set; }
        public int? RestaurantId { get; set; }

        public virtual Customer? Customer { get; set; }
        public virtual Restaurant? Restaurant { get; set; }
    }
}
