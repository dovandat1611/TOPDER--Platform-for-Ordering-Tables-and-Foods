using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.Menu
{
    public class CreateExcelMenuDto
    {
        [Required(ErrorMessage = "RestaurantId is required.")]
        public int RestaurantId { get; set; }

        [Required(ErrorMessage = "File is required.")]
        public IFormFile? File { get; set; }

    }
}
