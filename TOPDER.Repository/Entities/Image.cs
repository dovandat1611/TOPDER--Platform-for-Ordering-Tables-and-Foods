﻿using System;
using System.Collections.Generic;

namespace TOPDER.Repository.Entities
{
    public partial class Image
    {
        public int ImageId { get; set; }
        public int? RestaurantId { get; set; }
        public string? ImageUrl { get; set; }

        public virtual Restaurant? Restaurant { get; set; }
    }
}
