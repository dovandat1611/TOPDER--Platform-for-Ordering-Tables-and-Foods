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
using TOPDER.Service.Dtos.Contact;
using TOPDER.Service.Dtos.Feedback;
using TOPDER.Service.Dtos.Discount;
using TOPDER.Service.Dtos.Report;
using TOPDER.Service.Dtos.Notification;
using TOPDER.Service.Dtos.Wishlist;

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

            // CONTACT
            CreateMap<ContactDto, Contact>().ReverseMap();

            // FEEDBACK
            CreateMap<FeedbackDto, Feedback>().ReverseMap();

            CreateMap<Feedback, FeedbackCustomerDto>()
                .ForMember(dest => dest.CustomerImage, otp => otp.MapFrom(src => src.Customer != null ? src.Customer.Image : Is_Null.ISNULL))
                .ForMember(dest => dest.CustomerName, otp => otp.MapFrom(src => src.Customer != null ? src.Customer.Name : Is_Null.ISNULL));

            CreateMap<Feedback, FeedbackAdminDto>()
                .ForMember(dest => dest.CustomerName, otp => otp.MapFrom(src => src.Customer != null ? src.Customer.Name : Is_Null.ISNULL))
                .ForMember(dest => dest.RestaurantName, otp => otp.MapFrom(src => src.Restaurant != null ? src.Restaurant.NameRes : Is_Null.ISNULL));

            CreateMap<Feedback, FeedbackRestaurantDto>()
                .ForMember(dest => dest.CustomerName, otp => otp.MapFrom(src => src.Customer != null ? src.Customer.Name : Is_Null.ISNULL));

            CreateMap<Feedback, FeedbackHistoryDto>()
                .ForMember(dest => dest.RestaurantName,
                           opt => opt.MapFrom(src => src.Restaurant != null ? src.Restaurant.NameRes : Is_Null.ISNULL))
                .ForMember(dest => dest.RestaurantImage,
                           opt => opt.MapFrom(src => src.Restaurant != null ? src.Restaurant.Logo : Is_Null.ISNULL));

            // DISCOUNT
            CreateMap<DiscountDto, Discount>().ReverseMap();

            // REPORT
            CreateMap<ReportDto, Report>().ReverseMap();

            CreateMap<Report, ReportListDto>()
                .ForMember(dest => dest.ReportedByEmail,
                           opt => opt.MapFrom(src => src.ReportedByNavigation != null ? src.ReportedByNavigation.Email : Is_Null.ISNULL))
                .ForMember(dest => dest.ReportedOnEmail,
                           opt => opt.MapFrom(src => src.ReportedOnNavigation != null ? src.ReportedOnNavigation.Email : Is_Null.ISNULL));
            // NOTIFICATION
            CreateMap<NotificationDto, Notification>().ReverseMap();

            // WHISTLIST
            CreateMap<WishlistDto, Wishlist>().ReverseMap();
            CreateMap<Wishlist, UserWishlistDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src =>
                    src.Restaurant != null && src.Restaurant.CategoryRestaurant != null
                    ? src.Restaurant.CategoryRestaurant.CategoryRestaurantName
                    : Is_Null.ISNULL))
                .ForMember(dest => dest.NameRes, opt => opt.MapFrom(src =>
                    src.Restaurant != null
                    ? src.Restaurant.NameRes
                    : Is_Null.ISNULL))
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src =>
                    src.Restaurant != null && src.Restaurant.CategoryRestaurantId != null
                    ? src.Restaurant.CategoryRestaurantId.Value
                    : 0))
                .ForMember(dest => dest.TotalFeedbacks, opt => opt.MapFrom(src =>
                    src.Restaurant != null && src.Restaurant.Feedbacks != null
                    ? src.Restaurant.Feedbacks.Count()
                    : 0))
                .ForMember(dest => dest.Star, opt => opt.MapFrom(src =>
                    src.Restaurant != null && src.Restaurant.Feedbacks != null && src.Restaurant.Feedbacks.Any()
                    ? (int)Math.Round(src.Restaurant.Feedbacks.Average(r => r.Star.HasValue ? r.Star.Value : 0))
                    : 0));

        }
    }
}
