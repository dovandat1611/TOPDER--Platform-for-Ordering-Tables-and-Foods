using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.TableBookingSchedule
{
    public class CreateTableBookingScheduleDto
    {
        public List<int> TableIds { get; set; } = new List<int>();
        public int RestaurantId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string? Notes { get; set; }
    }
}
