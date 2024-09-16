using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.Blog
{
    public class NewBlogCustomerDto
    {
        public int BlogId { get; set; }
        public string? Image { get; set; }
        public string? Title { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}
