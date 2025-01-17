﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.Blog
{
    public class CreateBlogModel
    {
        public int? BloggroupId { get; set; }
        public int? AdminId { get; set; }
        public IFormFile? ImageFile { get; set; }
        public string? Image { get; set; }
        public string? Title { get; set; }
        public string Content { get; set; } = null!;
    }
}
