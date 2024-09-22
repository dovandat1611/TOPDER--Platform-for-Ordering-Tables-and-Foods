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
using TOPDER.Service.Dtos.RestaurantTable;
using TOPDER.Service.Dtos.CategoryRestaurant;
using TOPDER.Service.Dtos.Image;
using TOPDER.Service.Dtos.User;

namespace TOPDER.Service.Mapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
           // RESTAURANT 
            CreateMap<Restaurant, RestaurantHomeDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.CategoryRestaurant != null ? src.CategoryRestaurant.CategoryRestaurantName : Is_Null.ISNULL))
                .ForMember(dest => dest.TotalFeedbacks, opt => opt.MapFrom(src => src.Feedbacks != null ? src.Feedbacks.Count() : 0))
                .ForMember(dest => dest.Star, opt => opt.MapFrom(src =>
                    src.Feedbacks != null && src.Feedbacks.Any()
                        ? (int)Math.Round(src.Feedbacks.Average(r => r.Star ?? 0)) 
                        : 0
                ));

            CreateMap<CreateRestaurantRequest, Restaurant>();

            // CATEGORY MENU
            CreateMap<CategoryMenuDto, CategoryMenu>().ReverseMap();

            // CATEGORY RESTAURANT
            CreateMap<CategoryRestaurantDto, CategoryRestaurant>().ReverseMap();


            CreateMap<CreateCategoryMenuDto, CategoryMenu>().ReverseMap();

            // MENU
            CreateMap<Menu, MenuRestaurantDto>()
                .ForMember(dest => dest.CategoryMenuName,
                           otp => otp.MapFrom(src => src.CategoryMenu != null ? src.CategoryMenu.CategoryMenuName : Is_Null.ISNULL));

            CreateMap<Menu, MenuCustomerDto>()
                .ForMember(dest => dest.CategoryMenuName,
                           otp => otp.MapFrom(src => src.CategoryMenu != null ? src.CategoryMenu.CategoryMenuName : Is_Null.ISNULL));

            CreateMap<MenuDto, Menu>().ReverseMap();

            // RESTAURANT TABLE

            CreateMap<RestaurantTable, RestaurantTableCustomerDto>();
            CreateMap<RestaurantTable, RestaurantTableRestaurantDto>();
            CreateMap<RestaurantTableDto, RestaurantTable>().ReverseMap();

            // RESTAURANT IMAGE 
            CreateMap<ImageDto, Image>().ReverseMap();

            // BLOG GROUP
            CreateMap<BlogGroupDto, BlogGroup>().ReverseMap();

            // BLOG
            CreateMap<CreateBlogModel, Blog>().ReverseMap();

            CreateMap<UpdateBlogModel, Blog>().ReverseMap();

            CreateMap<Blog, NewBlogCustomerDto>();

            CreateMap<User, UserLoginDTO>()
    .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));

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
