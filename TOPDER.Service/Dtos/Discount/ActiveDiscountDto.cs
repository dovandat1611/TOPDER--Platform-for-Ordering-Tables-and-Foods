using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.Discount
{
    public class ActiveDiscountDto
    {
        public int Id { get; set; }
        public int RestaurantId { get; set; }
        public bool IsActive { get; set; }
    }

}
