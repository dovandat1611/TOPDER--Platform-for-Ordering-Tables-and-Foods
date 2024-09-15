using AutoMapper;
using TOPDER.Repository.Entities;
using TOPDER.Service.Dtos.Restaurant;
using System.Linq;

namespace TOPDER.Service.Mapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            //CreateMap<Restaurant, RestaurantHomeDTO>()
            //    .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.CategoryName : "Unknown"))
            //    .ForMember(dest => dest.TotalReviews, opt => opt.MapFrom(src => src.Reviews != null ? src.Reviews.Count() : 0))
            //    .ForMember(dest => dest.Star, opt => opt.MapFrom(src => src.Reviews != null && src.Reviews.Any() ? (int)src.Reviews.Average(r => r.Star) : 0));
            
            CreateMap<Restaurant, RestaurantHomeDTO>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.CategoryName : "Unknown"))
                .ForMember(dest => dest.TotalReviews, opt => opt.MapFrom(src => src.Reviews != null ? src.Reviews.Count() : 0))
                .ForMember(dest => dest.Star, opt => opt.MapFrom(src =>
                    src.Reviews != null && src.Reviews.Any()
                        ? (int)Math.Round(src.Reviews.Average(r => r.Star ?? 0)) // Handle nullable int with default value 0
                        : 0
                ));

            CreateMap<CreateRestaurantRequest, Restaurant>();
        }
    }
}
