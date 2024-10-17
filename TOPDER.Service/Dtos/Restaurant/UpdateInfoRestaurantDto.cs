﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.Restaurant
{
    public class UpdateInfoRestaurantDto
    {
        [Required(ErrorMessage = "Uid is required.")]
        public int Uid { get; set; }
        public int? CategoryRestaurantId { get; set; }
        public string NameOwner { get; set; } = null!;
        public string NameRes { get; set; } = null!;
        public string? Logo { get; set; }
        public IFormFile? File { get; set; }
        public TimeSpan OpenTime { get; set; }
        public TimeSpan CloseTime { get; set; }
        public int? ProvinceCity { get; set; }
        public int? District { get; set; }
        public int? Commune { get; set; }
        public string Address { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public int MaxCapacity { get; set; }
        public decimal Price { get; set; }
    }
}
