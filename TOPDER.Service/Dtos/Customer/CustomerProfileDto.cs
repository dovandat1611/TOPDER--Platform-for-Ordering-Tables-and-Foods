using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.Customer
{
    public class CustomerProfileDto
    {
        public int Uid { get; set; }
        public string? Name { get; set; } 
        public string? Phone { get; set; } 
        public string? Image { get; set; }
        public IFormFile? ImageFile { get; set; }
        public DateTime? Dob { get; set; }
        public string? Gender { get; set; }
    }
}
