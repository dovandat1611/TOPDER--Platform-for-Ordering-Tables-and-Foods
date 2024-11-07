using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.Order
{
    public class CreateRestaurantOrderTablesDto
    {
        public int OrderId { get; set; }
        public List<int> TableIds { get; set; } = new List<int>();
    }
}
