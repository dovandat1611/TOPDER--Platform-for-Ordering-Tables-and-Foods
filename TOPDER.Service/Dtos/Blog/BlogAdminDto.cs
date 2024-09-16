using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.Blog
{
    public class BlogAdminDto
    {
        public int BlogId { get; set; }
        public string BloggroupName { get; set; } = null!;
        public string? Image { get; set; }
        public string? Title { get; set; }
        public DateTime? CreateDate { get; set; }
        public string? Status { get; set; }
    }
}
