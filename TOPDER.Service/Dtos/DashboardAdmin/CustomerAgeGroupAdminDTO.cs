using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.DashboardAdmin
{
    public class CustomerAgeGroupAdminDTO
    {
        public int Under18 { get; set; }
        public int Between18And24 { get; set; }
        public int Between25And34 { get; set; }
        public int Between35And44 { get; set; }
        public int Between45And54 { get; set; }
        public int Above55 { get; set; }
    }
}
