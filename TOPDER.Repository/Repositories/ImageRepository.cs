using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Repository.Entities;
using TOPDER.Repository.IRepositories;

namespace TOPDER.Repository.Repositories
{
    public class ImageRepository : GenericRepository<Image>, IImageRepository
    {
        public ImageRepository(TopderDBContext dbContext) : base(dbContext) { }
    }
}
