﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace TOPDER.Repository.Entities
{
    public partial class TopderDBContext : DbContext
    {
        public TopderDBContext()
        {
        }

        public TopderDBContext(DbContextOptions<TopderDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Admin> Admins { get; set; } = null!;
        public virtual DbSet<Blog> Blogs { get; set; } = null!;
        public virtual DbSet<BlogGroup> BlogGroups { get; set; } = null!;
        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<Chat> Chats { get; set; } = null!;
        public virtual DbSet<ChatBox> ChatBoxes { get; set; } = null!;
        public virtual DbSet<Contact> Contacts { get; set; } = null!;
        public virtual DbSet<Customer> Customers { get; set; } = null!;
        public virtual DbSet<Discount> Discounts { get; set; } = null!;
        public virtual DbSet<ExternalLogin> ExternalLogins { get; set; } = null!;
        public virtual DbSet<Image> Images { get; set; } = null!;
        public virtual DbSet<Log> Logs { get; set; } = null!;
        public virtual DbSet<Menu> Menus { get; set; } = null!;
        public virtual DbSet<Notification> Notifications { get; set; } = null!;
        public virtual DbSet<Order> Orders { get; set; } = null!;
        public virtual DbSet<OrderDetail> OrderDetails { get; set; } = null!;
        public virtual DbSet<Report> Reports { get; set; } = null!;
        public virtual DbSet<Restaurant> Restaurants { get; set; } = null!;
        public virtual DbSet<Review> Reviews { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<WalletTransaction> WalletTransactions { get; set; } = null!;
        public virtual DbSet<Wishlist> Wishlists { get; set; } = null!;

//        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//        {
//            if (!optionsBuilder.IsConfigured)
//            {
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
//                optionsBuilder.UseSqlServer("Server=localhost;Database=TopderDB;user = sa; password = 123456;trustservercertificate=true");
//            }
//        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Admin>(entity =>
            {
                entity.HasKey(e => e.Uid)
                    .HasName("PK__Admin__DD701264DCC6F17B");

                entity.ToTable("Admin");

                entity.Property(e => e.Uid)
                    .ValueGeneratedNever()
                    .HasColumnName("uid");

                entity.Property(e => e.Dob)
                    .HasColumnType("date")
                    .HasColumnName("dob");

                entity.Property(e => e.Image)
                    .HasMaxLength(255)
                    .HasColumnName("image");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .HasColumnName("name");

                entity.Property(e => e.Phone)
                    .HasMaxLength(20)
                    .HasColumnName("phone");

                entity.HasOne(d => d.UidNavigation)
                    .WithOne(p => p.Admin)
                    .HasForeignKey<Admin>(d => d.Uid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Admin__uid__30F848ED");
            });

            modelBuilder.Entity<Blog>(entity =>
            {
                entity.ToTable("Blog");

                entity.Property(e => e.BlogId).HasColumnName("blog_id");

                entity.Property(e => e.AdminId).HasColumnName("admin_id");

                entity.Property(e => e.BloggroupId).HasColumnName("bloggroup_id");

                entity.Property(e => e.Content).HasColumnName("content");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("date")
                    .HasColumnName("create_date");

                entity.Property(e => e.Image)
                    .HasMaxLength(255)
                    .HasColumnName("image");

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .HasColumnName("status");

                entity.Property(e => e.Title)
                    .HasMaxLength(255)
                    .HasColumnName("title");

                entity.HasOne(d => d.Admin)
                    .WithMany(p => p.Blogs)
                    .HasForeignKey(d => d.AdminId)
                    .HasConstraintName("FK__Blog__admin_id__5070F446");

                entity.HasOne(d => d.Bloggroup)
                    .WithMany(p => p.Blogs)
                    .HasForeignKey(d => d.BloggroupId)
                    .HasConstraintName("FK__Blog__bloggroup___4F7CD00D");
            });

            modelBuilder.Entity<BlogGroup>(entity =>
            {
                entity.ToTable("Blog_Group");

                entity.Property(e => e.BloggroupId).HasColumnName("bloggroup_id");

                entity.Property(e => e.BloggroupName)
                    .HasMaxLength(255)
                    .HasColumnName("bloggroup_name");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Category");

                entity.Property(e => e.CategoryId).HasColumnName("category_id");

                entity.Property(e => e.CategoryName)
                    .HasMaxLength(255)
                    .HasColumnName("category_name");
            });

            modelBuilder.Entity<Chat>(entity =>
            {
                entity.ToTable("Chat");

                entity.Property(e => e.ChatId).HasColumnName("chat_id");

                entity.Property(e => e.ChatBoxId).HasColumnName("chat_box_id");

                entity.Property(e => e.ChatBy).HasColumnName("chat_by");

                entity.Property(e => e.ChatTime)
                    .HasColumnType("datetime")
                    .HasColumnName("chat_time")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Content).HasColumnName("content");

                entity.HasOne(d => d.ChatBox)
                    .WithMany(p => p.Chats)
                    .HasForeignKey(d => d.ChatBoxId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Chat__chat_box_i__70DDC3D8");

                entity.HasOne(d => d.ChatByNavigation)
                    .WithMany(p => p.Chats)
                    .HasForeignKey(d => d.ChatBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Chat__chat_by__71D1E811");
            });

            modelBuilder.Entity<ChatBox>(entity =>
            {
                entity.ToTable("ChatBox");

                entity.Property(e => e.ChatBoxId).HasColumnName("chat_box_id");

                entity.Property(e => e.CustomerId).HasColumnName("customer_id");

                entity.Property(e => e.RestaurantId).HasColumnName("restaurant_id");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.ChatBoxes)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ChatBox__custome__6C190EBB");

                entity.HasOne(d => d.Restaurant)
                    .WithMany(p => p.ChatBoxes)
                    .HasForeignKey(d => d.RestaurantId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ChatBox__restaur__6D0D32F4");
            });

            modelBuilder.Entity<Contact>(entity =>
            {
                entity.ToTable("Contact");

                entity.Property(e => e.ContactId).HasColumnName("contact_id");

                entity.Property(e => e.ContactDate)
                    .HasColumnType("date")
                    .HasColumnName("contact_date");

                entity.Property(e => e.Content).HasColumnName("content");

                entity.Property(e => e.Email)
                    .HasMaxLength(255)
                    .HasColumnName("email");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .HasColumnName("name");

                entity.Property(e => e.Phone)
                    .HasMaxLength(20)
                    .HasColumnName("phone");

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .HasColumnName("status");

                entity.Property(e => e.Topic)
                    .HasMaxLength(255)
                    .HasColumnName("topic");

                entity.Property(e => e.Uid).HasColumnName("uid");

                entity.HasOne(d => d.UidNavigation)
                    .WithMany(p => p.Contacts)
                    .HasForeignKey(d => d.Uid)
                    .HasConstraintName("FK__Contact__uid__534D60F1");
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.Uid)
                    .HasName("PK__Customer__DD7012648BB4406D");

                entity.ToTable("Customer");

                entity.Property(e => e.Uid)
                    .ValueGeneratedNever()
                    .HasColumnName("uid");

                entity.Property(e => e.Dob)
                    .HasColumnType("date")
                    .HasColumnName("dob");

                entity.Property(e => e.Gender)
                    .HasMaxLength(10)
                    .HasColumnName("gender");

                entity.Property(e => e.Image)
                    .HasMaxLength(255)
                    .HasColumnName("image");

                entity.Property(e => e.IsVerify).HasColumnName("is_verify");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .HasColumnName("name");

                entity.Property(e => e.OtpCode)
                    .HasMaxLength(10)
                    .HasColumnName("otp_code");

                entity.Property(e => e.Phone)
                    .HasMaxLength(20)
                    .HasColumnName("phone");

                entity.Property(e => e.WalletBalance)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("wallet_balance");

                entity.HasOne(d => d.UidNavigation)
                    .WithOne(p => p.Customer)
                    .HasForeignKey<Customer>(d => d.Uid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Customer__uid__35BCFE0A");
            });

            modelBuilder.Entity<Discount>(entity =>
            {
                entity.ToTable("Discount");

                entity.Property(e => e.DiscountId).HasColumnName("discount_id");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.DiscountPercentage)
                    .HasColumnType("decimal(5, 2)")
                    .HasColumnName("discount_percentage");

                entity.Property(e => e.EndDate)
                    .HasColumnType("date")
                    .HasColumnName("end_date");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("is_active")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Quantity).HasColumnName("quantity");

                entity.Property(e => e.RestaurantId).HasColumnName("restaurant_id");

                entity.Property(e => e.StartDate)
                    .HasColumnType("date")
                    .HasColumnName("start_date");

                entity.HasOne(d => d.Restaurant)
                    .WithMany(p => p.Discounts)
                    .HasForeignKey(d => d.RestaurantId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Discount__restau__59063A47");
            });

            modelBuilder.Entity<ExternalLogin>(entity =>
            {
                entity.ToTable("External_Logins");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AccessToken)
                    .HasMaxLength(255)
                    .HasColumnName("access_token");

                entity.Property(e => e.ExternalProvider)
                    .HasMaxLength(50)
                    .HasColumnName("external_provider");

                entity.Property(e => e.ExternalUserId)
                    .HasMaxLength(50)
                    .HasColumnName("external_user_id");

                entity.Property(e => e.Uid).HasColumnName("uid");

                entity.HasOne(d => d.UidNavigation)
                    .WithMany(p => p.ExternalLogins)
                    .HasForeignKey(d => d.Uid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__External_Lo__uid__2C3393D0");
            });

            modelBuilder.Entity<Image>(entity =>
            {
                entity.ToTable("Image");

                entity.Property(e => e.ImageId).HasColumnName("image_id");

                entity.Property(e => e.ImageUrl)
                    .HasMaxLength(255)
                    .HasColumnName("imageUrl");

                entity.Property(e => e.RestaurantId).HasColumnName("restaurant_id");

                entity.HasOne(d => d.Restaurant)
                    .WithMany(p => p.Images)
                    .HasForeignKey(d => d.RestaurantId)
                    .HasConstraintName("FK__Image__restauran__4AB81AF0");
            });

            modelBuilder.Entity<Log>(entity =>
            {
                entity.ToTable("Log");

                entity.Property(e => e.LogId).HasColumnName("log_id");

                entity.Property(e => e.Action)
                    .HasMaxLength(50)
                    .HasColumnName("action");

                entity.Property(e => e.LogType)
                    .HasMaxLength(50)
                    .HasColumnName("log_type");

                entity.Property(e => e.Note)
                    .HasColumnType("text")
                    .HasColumnName("note");

                entity.Property(e => e.RestaurantId).HasColumnName("restaurant_id");

                entity.Property(e => e.Timestamp)
                    .HasColumnType("datetime")
                    .HasColumnName("timestamp")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Restaurant)
                    .WithMany(p => p.Logs)
                    .HasForeignKey(d => d.RestaurantId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Log__restaurant___44FF419A");
            });

            modelBuilder.Entity<Menu>(entity =>
            {
                entity.ToTable("Menu");

                entity.Property(e => e.MenuId).HasColumnName("menu_id");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.DishName)
                    .HasMaxLength(255)
                    .HasColumnName("dish_name");

                entity.Property(e => e.Image)
                    .HasMaxLength(255)
                    .HasColumnName("image");

                entity.Property(e => e.Price)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("price");

                entity.Property(e => e.RestaurantId).HasColumnName("restaurant_id");

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .HasColumnName("status");

                entity.HasOne(d => d.Restaurant)
                    .WithMany(p => p.Menus)
                    .HasForeignKey(d => d.RestaurantId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Menu__restaurant__47DBAE45");
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.ToTable("Notification");

                entity.Property(e => e.NotificationId).HasColumnName("notification_id");

                entity.Property(e => e.Content).HasColumnName("content");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("created_date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsRead)
                    .HasColumnName("is_read")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Type)
                    .HasMaxLength(50)
                    .HasColumnName("type");

                entity.Property(e => e.Uid).HasColumnName("uid");

                entity.HasOne(d => d.UidNavigation)
                    .WithMany(p => p.Notifications)
                    .HasForeignKey(d => d.Uid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Notificatio__uid__7C4F7684");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Order");

                entity.Property(e => e.OrderId).HasColumnName("order_id");

                entity.Property(e => e.CancelledDate)
                    .HasColumnType("datetime")
                    .HasColumnName("cancelled_date");

                entity.Property(e => e.CompletedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("completed_date");

                entity.Property(e => e.ConfirmedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("confirmed_date");

                entity.Property(e => e.ContentPayment).HasColumnName("content_payment");

                entity.Property(e => e.ContentReservation).HasColumnName("content_reservation");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("created_date");

                entity.Property(e => e.CustomerId).HasColumnName("customer_id");

                entity.Property(e => e.DateReservation)
                    .HasColumnType("date")
                    .HasColumnName("date_reservation");

                entity.Property(e => e.DiscountId).HasColumnName("discount_id");

                entity.Property(e => e.NameReceiver)
                    .HasMaxLength(255)
                    .HasColumnName("name_receiver");

                entity.Property(e => e.NumberChild).HasColumnName("number_child");

                entity.Property(e => e.NumberPerson).HasColumnName("number_person");

                entity.Property(e => e.PaidDate)
                    .HasColumnType("datetime")
                    .HasColumnName("paid_date");

                entity.Property(e => e.PhoneReceiver)
                    .HasMaxLength(20)
                    .HasColumnName("phone_receiver");

                entity.Property(e => e.RestaurantId).HasColumnName("restaurant_id");

                entity.Property(e => e.StatusOrder)
                    .HasMaxLength(50)
                    .HasColumnName("status_order");

                entity.Property(e => e.StatusPayment)
                    .HasMaxLength(50)
                    .HasColumnName("status_payment");

                entity.Property(e => e.TimeReservation).HasColumnName("time_reservation");

                entity.Property(e => e.TotalPrice)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("total_price");

                entity.Property(e => e.TypeOrder)
                    .HasMaxLength(50)
                    .HasColumnName("type_order");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK__Order__customer___5BE2A6F2");

                entity.HasOne(d => d.Discount)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.DiscountId)
                    .HasConstraintName("FK__Order__discount___5DCAEF64");

                entity.HasOne(d => d.Restaurant)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.RestaurantId)
                    .HasConstraintName("FK__Order__restauran__5CD6CB2B");
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.ToTable("Order_Detail");

                entity.Property(e => e.OrderDetailId).HasColumnName("order_detail_id");

                entity.Property(e => e.MenuId).HasColumnName("menu_id");

                entity.Property(e => e.OrderId).HasColumnName("order_id");

                entity.Property(e => e.Price)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("price");

                entity.Property(e => e.Quantity).HasColumnName("quantity");

                entity.HasOne(d => d.Menu)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.MenuId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Order_Det__menu___619B8048");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Order_Det__order__60A75C0F");
            });

            modelBuilder.Entity<Report>(entity =>
            {
                entity.ToTable("Report");

                entity.Property(e => e.ReportId).HasColumnName("report_id");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("created_date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.ReportType)
                    .HasMaxLength(50)
                    .HasColumnName("report_type");

                entity.Property(e => e.ReportedBy).HasColumnName("reported_by");

                entity.Property(e => e.ReportedOn).HasColumnName("reported_on");

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .HasColumnName("status")
                    .HasDefaultValueSql("('Pending')");

                entity.HasOne(d => d.ReportedByNavigation)
                    .WithMany(p => p.ReportReportedByNavigations)
                    .HasForeignKey(d => d.ReportedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Report__reported__76969D2E");

                entity.HasOne(d => d.ReportedOnNavigation)
                    .WithMany(p => p.ReportReportedOnNavigations)
                    .HasForeignKey(d => d.ReportedOn)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Report__reported__778AC167");
            });

            modelBuilder.Entity<Restaurant>(entity =>
            {
                entity.HasKey(e => e.Uid)
                    .HasName("PK__Restaura__DD7012642ACDB1DD");

                entity.ToTable("Restaurant");

                entity.Property(e => e.Uid)
                    .ValueGeneratedNever()
                    .HasColumnName("uid");

                entity.Property(e => e.Address)
                    .HasMaxLength(255)
                    .HasColumnName("address");

                entity.Property(e => e.CancellationFeePercent)
                    .HasColumnType("decimal(5, 2)")
                    .HasColumnName("cancellation_fee_percent");

                entity.Property(e => e.CategoryId).HasColumnName("category_id");

                entity.Property(e => e.CloseTime).HasColumnName("close_time");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.Discount)
                    .HasMaxLength(255)
                    .HasColumnName("discount");

                entity.Property(e => e.FirstFeePercent)
                    .HasColumnType("decimal(5, 2)")
                    .HasColumnName("first_fee_percent");

                entity.Property(e => e.IsBookingEnabled)
                    .HasColumnName("is_booking_enabled")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Location)
                    .HasMaxLength(255)
                    .HasColumnName("location");

                entity.Property(e => e.Logo)
                    .HasMaxLength(255)
                    .HasColumnName("logo");

                entity.Property(e => e.NameOwner)
                    .HasMaxLength(255)
                    .HasColumnName("name_owner");

                entity.Property(e => e.NameRes)
                    .HasMaxLength(255)
                    .HasColumnName("name_res");

                entity.Property(e => e.OpenTime).HasColumnName("open_time");

                entity.Property(e => e.Phone)
                    .HasMaxLength(20)
                    .HasColumnName("phone");

                entity.Property(e => e.Price)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("price");

                entity.Property(e => e.ReturningFeePercent)
                    .HasColumnType("decimal(5, 2)")
                    .HasColumnName("returning_fee_percent");

                entity.Property(e => e.Subdescription).HasColumnName("subdescription");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Restaurants)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK__Restauran__categ__3C69FB99");

                entity.HasOne(d => d.UidNavigation)
                    .WithOne(p => p.Restaurant)
                    .HasForeignKey<Restaurant>(d => d.Uid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Restaurant__uid__3D5E1FD2");
            });

            modelBuilder.Entity<Review>(entity =>
            {
                entity.HasKey(e => e.RateId)
                    .HasName("PK__Review__75920B4252741B2D");

                entity.ToTable("Review");

                entity.Property(e => e.RateId).HasColumnName("rate_id");

                entity.Property(e => e.Content).HasColumnName("content");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("date")
                    .HasColumnName("create_date");

                entity.Property(e => e.CustomerId).HasColumnName("customer_id");

                entity.Property(e => e.RestaurantId).HasColumnName("restaurant_id");

                entity.Property(e => e.Star).HasColumnName("star");

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .HasColumnName("status");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Reviews)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK__Review__customer__6477ECF3");

                entity.HasOne(d => d.Restaurant)
                    .WithMany(p => p.Reviews)
                    .HasForeignKey(d => d.RestaurantId)
                    .HasConstraintName("FK__Review__restaura__656C112C");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role");

                entity.Property(e => e.RoleId).HasColumnName("role_id");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Uid)
                    .HasName("PK__User__DD7012646096B2E2");

                entity.ToTable("User");

                entity.HasIndex(e => e.Email, "UQ__User__AB6E6164BFC2920D")
                    .IsUnique();

                entity.Property(e => e.Uid).HasColumnName("uid");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("created_date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Email)
                    .HasMaxLength(255)
                    .HasColumnName("email");

                entity.Property(e => e.IsExternalLogin).HasColumnName("is_external_login");

                entity.Property(e => e.Password)
                    .HasMaxLength(255)
                    .HasColumnName("password");

                entity.Property(e => e.RoleId).HasColumnName("role_id");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__User__role_id__29572725");
            });

            modelBuilder.Entity<WalletTransaction>(entity =>
            {
                entity.HasKey(e => e.TransactionId)
                    .HasName("PK__WalletTr__85C600AF0F33D7DD");

                entity.ToTable("WalletTransaction");

                entity.Property(e => e.TransactionId).HasColumnName("transaction_id");

                entity.Property(e => e.CustomerId).HasColumnName("customer_id");

                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .HasColumnName("description");

                entity.Property(e => e.TransactionAmount)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("transaction_amount");

                entity.Property(e => e.TransactionDate)
                    .HasColumnType("datetime")
                    .HasColumnName("transaction_date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.TransactionType)
                    .HasMaxLength(50)
                    .HasColumnName("transaction_type");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.WalletTransactions)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__WalletTra__custo__412EB0B6");
            });

            modelBuilder.Entity<Wishlist>(entity =>
            {
                entity.ToTable("Wishlist");

                entity.Property(e => e.WishlistId).HasColumnName("wishlist_id");

                entity.Property(e => e.CustomerId).HasColumnName("customer_id");

                entity.Property(e => e.RestaurantId).HasColumnName("restaurant_id");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Wishlists)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK__Wishlist__custom__68487DD7");

                entity.HasOne(d => d.Restaurant)
                    .WithMany(p => p.Wishlists)
                    .HasForeignKey(d => d.RestaurantId)
                    .HasConstraintName("FK__Wishlist__restau__693CA210");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
