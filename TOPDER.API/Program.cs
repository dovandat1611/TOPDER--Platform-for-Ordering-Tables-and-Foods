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

// REPO: Scoped
builder.Services.AddScoped<IRestaurantRepository, RestaurantRepository>();

// Service: Transient
builder.Services.AddTransient<IRestaurantService, RestaurantService>();

// Other: ASK CHAT GPT
builder.Services.AddTransient<CloudinaryService>();
builder.Services.AddTransient<ISendMailService, SendMailService>();
builder.Services.AddScoped<IExcelService, ExcelService>();

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
