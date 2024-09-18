using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace TOPDER.Service.Dtos.RestaurantTable
{
    public class CreateExcelRestaurantTableDto
    {
        [Required(ErrorMessage = "RestaurantId is required.")]
        public int RestaurantId { get; set; }

        [Required(ErrorMessage = "File is required.")]
        public IFormFile? File { get; set; }  
    }
}