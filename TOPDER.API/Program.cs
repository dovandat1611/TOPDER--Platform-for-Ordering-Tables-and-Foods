using Microsoft.EntityFrameworkCore;
using Service.Services;
using TOPDER.Repository.Entities;
using TOPDER.Repository.IRepositories;
using TOPDER.Repository.Repositories;
using TOPDER.Service.Dtos.Email;
using TOPDER.Service.IServices;
using TOPDER.Service.Mapper;
using TOPDER.Service.Services;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<TopderDBContext>(options =>
  options.UseSqlServer(builder.Configuration.GetConnectionString("Dat_Connection"))
);

builder.Services.AddDistributedMemoryCache();

builder.Services.AddAutoMapper(typeof(AutoMapperProfile));


// Register your repositories and services as transient

// REPOSITORIES: Scoped
builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<IBlogGroupRepository, BlogGroupRepository>();
builder.Services.AddScoped<IBlogRepository, BlogRepository>();
builder.Services.AddScoped<ICategoryMenuRepository, CategoryMenuRepository>();
builder.Services.AddScoped<ICategoryRestaurantRepository, CategoryRestaurantRepository>();
builder.Services.AddScoped<ICategoryRoomRepository, CategoryRoomRepository>();
builder.Services.AddScoped<IChatBoxRepository, ChatBoxRepository>();
builder.Services.AddScoped<IChatRepository, ChatRepository>();
builder.Services.AddScoped<IContactRepository, ContactRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IDiscountRepository, DiscountRepository>();
builder.Services.AddScoped<IExternalLoginRepository, ExternalLoginRepository>();
builder.Services.AddScoped<IFeedbackRepository, FeedbackRepository>();
builder.Services.AddScoped<IImageRepository, ImageRepository>();
builder.Services.AddScoped<ILogRepository, LogRepository>();
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


// Service: Transient
builder.Services.AddTransient<IAdminService, AdminService>();
builder.Services.AddTransient<IBlogGroupService, BlogGroupService>();
builder.Services.AddTransient<IBlogService, BlogService>();
builder.Services.AddTransient<ICategoryMenuService, CategoryMenuService>();
builder.Services.AddTransient<ICategoryRestaurantService, CategoryRestaurantService>();
builder.Services.AddTransient<ICategoryRoomService, CategoryRoomService>();
builder.Services.AddTransient<IChatBoxService, ChatBoxService>();
builder.Services.AddTransient<IChatService, ChatService>();
builder.Services.AddTransient<IContactService, ContactService>();
builder.Services.AddTransient<ICustomerService, CustomerService>();
builder.Services.AddTransient<IDashboardService, DashboardService>();
builder.Services.AddTransient<IDiscountService, DiscountService>();
builder.Services.AddTransient<IExternalLoginService, ExternalLoginService>();
builder.Services.AddTransient<IFeedbackService, FeedbackService>();
builder.Services.AddTransient<IImageService, ImageService>();
builder.Services.AddTransient<ILogService, LogService>();
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



// Other: ASK CHAT GPT
builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();
builder.Services.AddScoped<ISendMailService, SendMailService>();
builder.Services.AddTransient<IExcelService, ExcelService>();
builder.Services.AddScoped<IPaymentGatewayService, PaymentGatewayService>();


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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
