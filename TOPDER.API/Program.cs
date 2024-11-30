using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Service.Services;
using System.Text;
using TOPDER.Repository.Entities;
using TOPDER.Repository.IRepositories;
using TOPDER.Repository.Repositories;
using TOPDER.Service.Dtos.Email;
using TOPDER.Service.Hubs;
using TOPDER.Service.IServices;
using TOPDER.Service.Mapper;
using TOPDER.Service.Services;
using TOPDER.Service.Utils;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"], 
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])) 
        };
    })
    .AddGoogle(options =>
    {
        var googleSettings = builder.Configuration.GetSection("Google");
        options.ClientId = googleSettings["ClientId"];
        options.ClientSecret = googleSettings["ClientSecret"];
        options.SaveTokens = true;
    });


builder.Services.AddControllers()
    .AddJsonOptions(opt =>
    {
        opt.JsonSerializerOptions.ReferenceHandler =
            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations(); 
});


builder.Services.AddDbContext<TopderDBContext>(options =>
  options.UseSqlServer(builder.Configuration.GetConnectionString("Dat_Connection"))
);

builder.Services.AddDistributedMemoryCache();

builder.Services.AddAutoMapper(typeof(AutoMapperProfile));


// ADD SIGNALR
builder.Services.AddSignalR();

// Register your repositories and services as transient

// REPOSITORIES: Scoped
builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<IBlogGroupRepository, BlogGroupRepository>();
builder.Services.AddScoped<IBlogRepository, BlogRepository>();
builder.Services.AddScoped<ICategoryMenuRepository, CategoryMenuRepository>();
builder.Services.AddScoped<ICategoryRestaurantRepository, CategoryRestaurantRepository>();
builder.Services.AddScoped<IChatBoxRepository, ChatBoxRepository>();
builder.Services.AddScoped<IChatRepository, ChatRepository>();
builder.Services.AddScoped<IContactRepository, ContactRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IDiscountRepository, DiscountRepository>();
builder.Services.AddScoped<IExternalLoginRepository, ExternalLoginRepository>();
builder.Services.AddScoped<IFeedbackRepository, FeedbackRepository>();
builder.Services.AddScoped<IFeedbackReplyRepository, FeedbackReplyRepository>();
builder.Services.AddScoped<IImageRepository, ImageRepository>();
builder.Services.AddScoped<IMenuRepository, MenuRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IOrderMenuRepository, OrderMenuRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderTableRepository, OrderTableRepository>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<IRestaurantRepository, RestaurantRepository>();
builder.Services.AddScoped<IRestaurantRoomRepository, RestaurantRoomRepository>();
builder.Services.AddScoped<IRestaurantTableRepository, RestaurantTableRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IWalletRepository, WalletRepository>();
builder.Services.AddScoped<IWalletTransactionRepository, WalletTransactionRepository>();
builder.Services.AddScoped<IWishlistRepository, WishlistRepository>();
builder.Services.AddScoped<IDiscountMenuRepository, DiscountMenuRepository>();
builder.Services.AddScoped<IUserOtpRepository, UserOtpRepository>();
builder.Services.AddScoped<ITableBookingScheduleRepository, TableBookingScheduleRepository>();
builder.Services.AddScoped<IAdvertisementPricingRepository, AdvertisementPricingRepository>();
builder.Services.AddScoped<IBookingAdvertisementRepository, BookingAdvertisementRepository>();
builder.Services.AddScoped<IPolicySystemRepository, PolicySystemRepository>();
builder.Services.AddScoped<IRestaurantPolicyRepository, RestaurantPolicyRepository>();



// Service: Transient
builder.Services.AddTransient<IAdminService, AdminService>();
builder.Services.AddTransient<IBlogGroupService, BlogGroupService>();
builder.Services.AddTransient<IBlogService, BlogService>();
builder.Services.AddTransient<ICategoryMenuService, CategoryMenuService>();
builder.Services.AddTransient<ICategoryRestaurantService, CategoryRestaurantService>();
builder.Services.AddTransient<IChatBoxService, ChatBoxService>();
builder.Services.AddTransient<IChatService, ChatService>();
builder.Services.AddTransient<IContactService, ContactService>();
builder.Services.AddTransient<ICustomerService, CustomerService>();
builder.Services.AddTransient<IDashboardService, DashboardService>();
builder.Services.AddTransient<IDiscountService, DiscountService>();
builder.Services.AddTransient<IExternalLoginService, ExternalLoginService>();
builder.Services.AddTransient<IFeedbackService, FeedbackService>();
builder.Services.AddTransient<IFeedbackReplyService, FeedbackReplyService>();
builder.Services.AddTransient<IImageService, ImageService>();
builder.Services.AddTransient<IMenuService, MenuService>();
builder.Services.AddTransient<INotificationService, NotificationService>();
builder.Services.AddTransient<IOrderMenuService, OrderMenuService>();
builder.Services.AddTransient<IOrderService, OrderService>();
builder.Services.AddTransient<IOrderTableService, OrderTableService>();
builder.Services.AddTransient<IReportService, ReportService>();
builder.Services.AddTransient<IRestaurantRoomService, RestaurantRoomService>();
builder.Services.AddTransient<IRestaurantService, RestaurantService>();
builder.Services.AddTransient<IRestaurantTableService, RestaurantTableService>();
builder.Services.AddTransient<IRoleService, RoleService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IWalletService, WalletService>();
builder.Services.AddTransient<IWalletTransactionService, WalletTransactionService>();
builder.Services.AddTransient<IWishlistService, WishlistService>();
builder.Services.AddTransient<IWishlistService, WishlistService>();
builder.Services.AddTransient<ITableBookingScheduleService, TableBookingScheduleService>();
builder.Services.AddTransient<IBookingAdvertisementService, BookingAdvertisementService>();
builder.Services.AddTransient<IAdvertisementPricingService, AdvertisementPricingService>();
builder.Services.AddTransient<IPolicySystemService, PolicySystemService>();
builder.Services.AddTransient<IRestaurantPolicyService, RestaurantPolicyService>();


// Other: ASK CHAT GPT
builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();
builder.Services.AddScoped<ISendMailService, SendMailService>();
builder.Services.AddTransient<IExcelService, ExcelService>();
builder.Services.AddScoped<IPaymentGatewayService, PaymentGatewayService>();
builder.Services.AddSingleton<JwtHelper>();
builder.Services.AddScoped<IIdentityService, IdentityService>();
builder.Services.AddSingleton<AppHub>();


// ADD CORS
builder.Services.AddCors();


// Configure MailSettings
var mailSettingsSection = builder.Configuration.GetSection("MailSettings");
builder.Services.Configure<MailSettings>(mailSettingsSection); // register for dependency 

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(
    builder =>
    {
        builder.WithOrigins("http://127.0.0.1:5500", "http://localhost:3000", "https://topder.vercel.app")  // Thêm nhiều nguồn nếu cần
       .AllowAnyHeader()
       .AllowAnyMethod()
       .AllowCredentials();
    });

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapHub<AppHub>("/signalR");

app.Run();
