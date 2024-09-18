using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.Image
{
    public class ImageDto
    {   
        public int ImageId { get; set; }
        public int? RestaurantId { get; set; }
        public string? ImageUrl { get; set; }
    }
}
