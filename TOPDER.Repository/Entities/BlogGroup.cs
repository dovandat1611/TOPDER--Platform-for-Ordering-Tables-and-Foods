﻿using System;
using System.Collections.Generic;

namespace TOPDER.Repository.Entities
{
    public partial class BlogGroup
    {
        public BlogGroup()
        {
            Blogs = new HashSet<Blog>();
        }

        public int BloggroupId { get; set; }
        public string BloggroupName { get; set; } = null!;

        public virtual ICollection<Blog> Blogs { get; set; }
    }
}
