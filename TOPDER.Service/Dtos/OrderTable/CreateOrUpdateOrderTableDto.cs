using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.OrderTable
{
    public class CreateOrUpdateOrderTableDto
    {
        public int OrderTableId { get; set; }
        public int OrderId { get; set; }
        public int TableId { get; set; }
    }
}
