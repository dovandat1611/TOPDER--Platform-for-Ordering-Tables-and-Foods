using AutoMapper;
using TOPDER.Repository.Entities;
using TOPDER.Service.Dtos.Restaurant;
using System.Linq;
using TOPDER.Service.Dtos.Customer;
using TOPDER.Service.Dtos.BlogGroup;
using TOPDER.Service.Dtos.Blog;

namespace TOPDER.Service.Mapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
           // RESTAURANT 
            CreateMap<Restaurant, RestaurantHomeDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.CategoryRestaurant != null ? src.CategoryRestaurant.CategoryRestaurantName : "Unknown"))
                .ForMember(dest => dest.TotalReviews, opt => opt.MapFrom(src => src.Reviews != null ? src.Reviews.Count() : 0))
                .ForMember(dest => dest.Star, opt => opt.MapFrom(src =>
                    src.Reviews != null && src.Reviews.Any()
                        ? (int)Math.Round(src.Reviews.Average(r => r.Star ?? 0)) 
                        : 0
                ));
            CreateMap<CreateRestaurantRequest, Restaurant>();

            // BLOG GROUP
            CreateMap<BlogGroupDto, BlogGroup>().ReverseMap();


            // BLOG
            CreateMap<CreateBlogModel, Blog>().ReverseMap();

            CreateMap<UpdateBlogModel, Blog>().ReverseMap();

            CreateMap<Blog, NewBlogCustomerDto>();

            CreateMap<Blog, BlogAdminDto>()
                .ForMember(dest => dest.BloggroupName,
                           otp => otp.MapFrom(src => src.Bloggroup != null ? src.Bloggroup.BloggroupName : "N/A"));

            CreateMap<Blog, BlogDetailCustomerDto>()
                .ForMember(dest => dest.BloggroupName,
                           otp => otp.MapFrom(src => src.Bloggroup != null ? src.Bloggroup.BloggroupName : "N/A"))
                .ForMember(dest => dest.CreatBy_Name,
                           otp => otp.MapFrom(src => src.Admin != null ? src.Admin.Name : "TOPDER"));

            CreateMap<Blog, BlogListCustomerDto>()
                .ForMember(dest => dest.BloggroupName,
                           otp => otp.MapFrom(src => src.Bloggroup != null ? src.Bloggroup.BloggroupName : "N/A"))
                .ForMember(dest => dest.CreatBy_Name,
                           otp => otp.MapFrom(src => src.Admin != null ? src.Admin.Name : "TOPDER"))
                .ForMember(dest => dest.CreatBy_Image,
                           otp => otp.MapFrom(src => src.Admin != null ? src.Admin.Image : "TOPDER"));

            // CUSTOMER
            CreateMap<Customer, CustomerInfoDto>()
                .ForMember(dest => dest.Email, otp => otp.MapFrom(src => src.UidNavigation.Email));

        }
    }
}
