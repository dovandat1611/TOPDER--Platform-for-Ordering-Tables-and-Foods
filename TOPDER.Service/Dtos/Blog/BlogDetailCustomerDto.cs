using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.Blog
{
    public class BlogDetailCustomerDto
    {
        public int BlogId { get; set; }
        public int? BloggroupId { get; set; }
        public string BloggroupName { get; set; } = null!;
        public string? CreatBy_Name { get; set; }
        public string? Image { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}
