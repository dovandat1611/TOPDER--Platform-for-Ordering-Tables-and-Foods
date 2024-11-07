using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Service.Dtos.BlogGroup;

namespace TOPDER.Service.Dtos.Blog
{
    public class BlogDetailDto
    {
        public BlogDetailCustomerDto blogListCustomer { get; set; }  = null!;
        public List<NewBlogCustomerDto> newBlogCustomers { get; set; } = null!;
        public List<BlogGroupDto> blogGroups { get; set; } = null!;
    }
}
