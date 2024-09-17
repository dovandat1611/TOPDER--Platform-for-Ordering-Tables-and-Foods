using AutoMapper;
using TOPDER.Repository.Entities;
using TOPDER.Service.Dtos.Restaurant;
using System.Linq;
using TOPDER.Service.Dtos.Customer;
using TOPDER.Service.Dtos.BlogGroup;
using TOPDER.Service.Dtos.Blog;
using TOPDER.Service.Dtos.CategoryMenu;
using TOPDER.Service.Dtos.Menu;
using static TOPDER.Service.Common.ServiceDefinitions.Constants;

namespace TOPDER.Service.Mapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
           // RESTAURANT 
            CreateMap<Restaurant, RestaurantHomeDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.CategoryRestaurant != null ? src.CategoryRestaurant.CategoryRestaurantName : Is_Null.ISNULL))
                .ForMember(dest => dest.TotalReviews, opt => opt.MapFrom(src => src.Reviews != null ? src.Reviews.Count() : 0))
                .ForMember(dest => dest.Star, opt => opt.MapFrom(src =>
                    src.Reviews != null && src.Reviews.Any()
                        ? (int)Math.Round(src.Reviews.Average(r => r.Star ?? 0)) 
                        : 0
                ));

            CreateMap<CreateRestaurantRequest, Restaurant>();

            // CATEGORY MENU
            CreateMap<CategoryMenuDto, CategoryMenu>().ReverseMap();

            CreateMap<CreateCategoryMenuDto, CategoryMenu>().ReverseMap();

            // MENU
            CreateMap<Menu, MenuRestaurantDto>()
                .ForMember(dest => dest.CategoryMenuName,
                           otp => otp.MapFrom(src => src.CategoryMenu != null ? src.CategoryMenu.CategoryMenuName : Is_Null.ISNULL));

            CreateMap<Menu, MenuCustomerDto>()
                .ForMember(dest => dest.CategoryMenuName,
                           otp => otp.MapFrom(src => src.CategoryMenu != null ? src.CategoryMenu.CategoryMenuName : Is_Null.ISNULL));

            CreateMap<MenuDto, Menu>().ReverseMap();

            // BLOG GROUP
            CreateMap<BlogGroupDto, BlogGroup>().ReverseMap();

            // BLOG
            CreateMap<CreateBlogModel, Blog>().ReverseMap();

            CreateMap<UpdateBlogModel, Blog>().ReverseMap();

            CreateMap<Blog, NewBlogCustomerDto>();

            CreateMap<Blog, BlogAdminDto>()
                .ForMember(dest => dest.BloggroupName,
                           otp => otp.MapFrom(src => src.Bloggroup != null ? src.Bloggroup.BloggroupName : Is_Null.ISNULL));

            CreateMap<Blog, BlogDetailCustomerDto>()
                .ForMember(dest => dest.BloggroupName,
                           otp => otp.MapFrom(src => src.Bloggroup != null ? src.Bloggroup.BloggroupName : Is_Null.ISNULL))
                .ForMember(dest => dest.CreatBy_Name,
                           otp => otp.MapFrom(src => src.Admin != null ? src.Admin.Name : Is_Null.ISNULL));

            CreateMap<Blog, BlogListCustomerDto>()
                .ForMember(dest => dest.BloggroupName,
                           otp => otp.MapFrom(src => src.Bloggroup != null ? src.Bloggroup.BloggroupName : Is_Null.ISNULL))
                .ForMember(dest => dest.CreatBy_Name,
                           otp => otp.MapFrom(src => src.Admin != null ? src.Admin.Name : Is_Null.ISNULL))
                .ForMember(dest => dest.CreatBy_Image,
                           otp => otp.MapFrom(src => src.Admin != null ? src.Admin.Image : Is_Null.ISNULL));

            // CUSTOMER
            CreateMap<Customer, CustomerInfoDto>()
                .ForMember(dest => dest.Email, otp => otp.MapFrom(src => src.UidNavigation.Email));

        }
    }
}
