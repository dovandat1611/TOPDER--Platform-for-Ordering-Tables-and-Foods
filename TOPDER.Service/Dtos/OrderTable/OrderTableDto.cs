using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.OrderTable
{
    public class OrderTableDto
    {
        public int OrderTableId { get; set; }
        public int OrderId { get; set; }
        public int TableId { get; set; }
        public int? RoomId { get; set; }
        public string? RoomName { get; set; }
        public string TableName { get; set; } = null!;
        public int MaxCapacity { get; set; }
    }
}
