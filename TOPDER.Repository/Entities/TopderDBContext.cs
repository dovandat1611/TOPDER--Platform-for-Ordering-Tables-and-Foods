using System;
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
        public virtual DbSet<CategoryMenu> CategoryMenus { get; set; } = null!;
        public virtual DbSet<CategoryRestaurant> CategoryRestaurants { get; set; } = null!;
        public virtual DbSet<CategoryRoom> CategoryRooms { get; set; } = null!;
        public virtual DbSet<Chat> Chats { get; set; } = null!;
        public virtual DbSet<ChatBox> ChatBoxes { get; set; } = null!;
        public virtual DbSet<Contact> Contacts { get; set; } = null!;
        public virtual DbSet<Customer> Customers { get; set; } = null!;
        public virtual DbSet<Discount> Discounts { get; set; } = null!;
        public virtual DbSet<DiscountMenu> DiscountMenus { get; set; } = null!;
        public virtual DbSet<ExternalLogin> ExternalLogins { get; set; } = null!;
        public virtual DbSet<Feedback> Feedbacks { get; set; } = null!;
        public virtual DbSet<Image> Images { get; set; } = null!;
        public virtual DbSet<Log> Logs { get; set; } = null!;
        public virtual DbSet<Menu> Menus { get; set; } = null!;
        public virtual DbSet<Notification> Notifications { get; set; } = null!;
        public virtual DbSet<Order> Orders { get; set; } = null!;
        public virtual DbSet<OrderMenu> OrderMenus { get; set; } = null!;
        public virtual DbSet<OrderTable> OrderTables { get; set; } = null!;
        public virtual DbSet<Report> Reports { get; set; } = null!;
        public virtual DbSet<Restaurant> Restaurants { get; set; } = null!;
        public virtual DbSet<RestaurantRoom> RestaurantRooms { get; set; } = null!;
        public virtual DbSet<RestaurantTable> RestaurantTables { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<TableBookingSchedule> TableBookingSchedules { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<UserOtp> UserOtps { get; set; } = null!;
        public virtual DbSet<Wallet> Wallets { get; set; } = null!;
        public virtual DbSet<WalletTransaction> WalletTransactions { get; set; } = null!;
        public virtual DbSet<Wishlist> Wishlists { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Admin>(entity =>
            {
                entity.HasKey(e => e.Uid)
                    .HasName("PK__Admin__DD701264A1F39EFE");

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
                    .HasConstraintName("FK__Admin__uid__36B12243");
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
                    .HasConstraintName("FK__Blog__admin_id__71D1E811");

                entity.HasOne(d => d.Bloggroup)
                    .WithMany(p => p.Blogs)
                    .HasForeignKey(d => d.BloggroupId)
                    .HasConstraintName("FK__Blog__bloggroup___70DDC3D8");
            });

            modelBuilder.Entity<BlogGroup>(entity =>
            {
                entity.ToTable("Blog_Group");

                entity.Property(e => e.BloggroupId).HasColumnName("bloggroup_id");

                entity.Property(e => e.BloggroupName)
                    .HasMaxLength(255)
                    .HasColumnName("bloggroup_name");
            });

            modelBuilder.Entity<CategoryMenu>(entity =>
            {
                entity.ToTable("Category_Menu");

                entity.Property(e => e.CategoryMenuId).HasColumnName("category_menu_id");

                entity.Property(e => e.CategoryMenuName)
                    .HasMaxLength(255)
                    .HasColumnName("category_menu_name");

                entity.Property(e => e.RestaurantId).HasColumnName("restaurant_id");

                entity.HasOne(d => d.Restaurant)
                    .WithMany(p => p.CategoryMenus)
                    .HasForeignKey(d => d.RestaurantId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Category___resta__656C112C");
            });

            modelBuilder.Entity<CategoryRestaurant>(entity =>
            {
                entity.ToTable("Category_Restaurant");

                entity.Property(e => e.CategoryRestaurantId).HasColumnName("category_restaurant_id");

                entity.Property(e => e.CategoryRestaurantName)
                    .HasMaxLength(255)
                    .HasColumnName("category_restaurant_name");
            });

            modelBuilder.Entity<CategoryRoom>(entity =>
            {
                entity.ToTable("Category_Room");

                entity.Property(e => e.CategoryRoomId).HasColumnName("category_room_id");

                entity.Property(e => e.CategoryName)
                    .HasMaxLength(255)
                    .HasColumnName("category_name");

                entity.Property(e => e.RestaurantId).HasColumnName("restaurant_id");

                entity.HasOne(d => d.Restaurant)
                    .WithMany(p => p.CategoryRooms)
                    .HasForeignKey(d => d.RestaurantId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Category___resta__49C3F6B7");
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
                    .HasConstraintName("FK__Chat__chat_box_i__1EA48E88");

                entity.HasOne(d => d.ChatByNavigation)
                    .WithMany(p => p.Chats)
                    .HasForeignKey(d => d.ChatBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Chat__chat_by__1F98B2C1");
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
                    .HasConstraintName("FK__ChatBox__custome__19DFD96B");

                entity.HasOne(d => d.Restaurant)
                    .WithMany(p => p.ChatBoxes)
                    .HasForeignKey(d => d.RestaurantId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ChatBox__restaur__1AD3FDA4");
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
                    .HasConstraintName("FK__Contact__uid__74AE54BC");
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.Uid)
                    .HasName("PK__Customer__DD70126454EB552B");

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

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .HasColumnName("name");

                entity.Property(e => e.Phone)
                    .HasMaxLength(20)
                    .HasColumnName("phone");

                entity.HasOne(d => d.UidNavigation)
                    .WithOne(p => p.Customer)
                    .HasForeignKey<Customer>(d => d.Uid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Customer__uid__398D8EEE");
            });

            modelBuilder.Entity<Discount>(entity =>
            {
                entity.ToTable("Discount");

                entity.Property(e => e.DiscountId).HasColumnName("discount_id");

                entity.Property(e => e.ApplicableTo)
                    .HasMaxLength(50)
                    .HasColumnName("applicable_to");

                entity.Property(e => e.ApplyType)
                    .HasMaxLength(50)
                    .HasColumnName("apply_type");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.DiscountName)
                    .HasMaxLength(100)
                    .HasColumnName("discount_name");

                entity.Property(e => e.DiscountPercentage)
                    .HasColumnType("decimal(5, 2)")
                    .HasColumnName("discount_percentage");

                entity.Property(e => e.EndDate)
                    .HasColumnType("datetime")
                    .HasColumnName("end_date");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("is_active")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.MaxOrderValue)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("max_order_value");

                entity.Property(e => e.MinOrderValue)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("min_order_value");

                entity.Property(e => e.Quantity).HasColumnName("quantity");

                entity.Property(e => e.RestaurantId).HasColumnName("restaurant_id");

                entity.Property(e => e.Scope)
                    .HasMaxLength(50)
                    .HasColumnName("scope");

                entity.Property(e => e.StartDate)
                    .HasColumnType("datetime")
                    .HasColumnName("start_date");

                entity.HasOne(d => d.Restaurant)
                    .WithMany(p => p.Discounts)
                    .HasForeignKey(d => d.RestaurantId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Discount__restau__7A672E12");
            });

            modelBuilder.Entity<DiscountMenu>(entity =>
            {
                entity.ToTable("Discount_Menu");

                entity.Property(e => e.DiscountMenuId).HasColumnName("discount_menu_id");

                entity.Property(e => e.DiscountId).HasColumnName("discount_id");

                entity.Property(e => e.DiscountMenuPercentage)
                    .HasColumnType("decimal(5, 2)")
                    .HasColumnName("discount_menu_percentage");

                entity.Property(e => e.MenuId).HasColumnName("menu_id");

                entity.HasOne(d => d.Discount)
                    .WithMany(p => p.DiscountMenus)
                    .HasForeignKey(d => d.DiscountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Discount___disco__7E37BEF6");

                entity.HasOne(d => d.Menu)
                    .WithMany(p => p.DiscountMenus)
                    .HasForeignKey(d => d.MenuId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Discount___menu___7F2BE32F");
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
                    .HasConstraintName("FK__External_Lo__uid__31EC6D26");
            });

            modelBuilder.Entity<Feedback>(entity =>
            {
                entity.ToTable("Feedback");

                entity.Property(e => e.FeedbackId).HasColumnName("feedback_id");

                entity.Property(e => e.Content).HasColumnName("content");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("create_date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.CustomerId).HasColumnName("customer_id");

                entity.Property(e => e.OrderId).HasColumnName("order_id");

                entity.Property(e => e.RestaurantId).HasColumnName("restaurant_id");

                entity.Property(e => e.Star).HasColumnName("star");

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .HasColumnName("status");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Feedbacks)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK__Feedback__custom__123EB7A3");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.Feedbacks)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Feedback__order___114A936A");

                entity.HasOne(d => d.Restaurant)
                    .WithMany(p => p.Feedbacks)
                    .HasForeignKey(d => d.RestaurantId)
                    .HasConstraintName("FK__Feedback__restau__1332DBDC");
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
                    .HasConstraintName("FK__Image__restauran__6C190EBB");
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

                entity.Property(e => e.Note).HasColumnName("note");

                entity.Property(e => e.Timestamp)
                    .HasColumnType("datetime")
                    .HasColumnName("timestamp")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Uid).HasColumnName("uid");

                entity.HasOne(d => d.UidNavigation)
                    .WithMany(p => p.Logs)
                    .HasForeignKey(d => d.Uid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Log__uid__628FA481");
            });

            modelBuilder.Entity<Menu>(entity =>
            {
                entity.ToTable("Menu");

                entity.Property(e => e.MenuId).HasColumnName("menu_id");

                entity.Property(e => e.CategoryMenuId).HasColumnName("category_menu_id");

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

                entity.HasOne(d => d.CategoryMenu)
                    .WithMany(p => p.Menus)
                    .HasForeignKey(d => d.CategoryMenuId)
                    .HasConstraintName("FK__Menu__category_m__68487DD7");

                entity.HasOne(d => d.Restaurant)
                    .WithMany(p => p.Menus)
                    .HasForeignKey(d => d.RestaurantId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Menu__restaurant__693CA210");
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.ToTable("Notification");

                entity.Property(e => e.NotificationId).HasColumnName("notification_id");

                entity.Property(e => e.Content).HasColumnName("content");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at")
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
                    .HasConstraintName("FK__Notificatio__uid__29221CFB");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Order");

                entity.Property(e => e.OrderId).HasColumnName("order_id");

                entity.Property(e => e.CancelReason).HasColumnName("cancel_reason");

                entity.Property(e => e.CancelledAt)
                    .HasColumnType("datetime")
                    .HasColumnName("cancelled_at");

                entity.Property(e => e.CategoryRoomId).HasColumnName("category_room_id");

                entity.Property(e => e.CompletedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("completed_at");

                entity.Property(e => e.ConfirmedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("confirmed_at");

                entity.Property(e => e.ContentPayment).HasColumnName("content_payment");

                entity.Property(e => e.ContentReservation).HasColumnName("content_reservation");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at");

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

                entity.Property(e => e.PaidAt)
                    .HasColumnType("datetime")
                    .HasColumnName("paid_at");

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

                entity.Property(e => e.TotalAmount)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("total_amount");

                entity.Property(e => e.TypeOrder)
                    .HasMaxLength(50)
                    .HasColumnName("type_order");

                entity.HasOne(d => d.CategoryRoom)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.CategoryRoomId)
                    .HasConstraintName("FK__Order__category___04E4BC85");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK__Order__customer___02084FDA");

                entity.HasOne(d => d.Discount)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.DiscountId)
                    .HasConstraintName("FK__Order__discount___03F0984C");

                entity.HasOne(d => d.Restaurant)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.RestaurantId)
                    .HasConstraintName("FK__Order__restauran__02FC7413");
            });

            modelBuilder.Entity<OrderMenu>(entity =>
            {
                entity.ToTable("Order_Menu");

                entity.Property(e => e.OrderMenuId).HasColumnName("order_menu_id");

                entity.Property(e => e.MenuId).HasColumnName("menu_id");

                entity.Property(e => e.OrderId).HasColumnName("order_id");

                entity.Property(e => e.Price)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("price");

                entity.Property(e => e.Quantity).HasColumnName("quantity");

                entity.HasOne(d => d.Menu)
                    .WithMany(p => p.OrderMenus)
                    .HasForeignKey(d => d.MenuId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Order_Men__menu___0C85DE4D");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderMenus)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Order_Men__order__0B91BA14");
            });

            modelBuilder.Entity<OrderTable>(entity =>
            {
                entity.ToTable("Order_Table");

                entity.Property(e => e.OrderTableId).HasColumnName("order_table_id");

                entity.Property(e => e.OrderId).HasColumnName("order_id");

                entity.Property(e => e.TableId).HasColumnName("table_id");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderTables)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Order_Tab__order__07C12930");

                entity.HasOne(d => d.Table)
                    .WithMany(p => p.OrderTables)
                    .HasForeignKey(d => d.TableId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Order_Tab__table__08B54D69");
            });

            modelBuilder.Entity<Report>(entity =>
            {
                entity.ToTable("Report");

                entity.Property(e => e.ReportId).HasColumnName("report_id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.ReportType)
                    .HasMaxLength(50)
                    .HasColumnName("report_type");

                entity.Property(e => e.ReportedBy).HasColumnName("reported_by");

                entity.Property(e => e.ReportedOn).HasColumnName("reported_on");

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .HasColumnName("status");

                entity.HasOne(d => d.ReportedByNavigation)
                    .WithMany(p => p.ReportReportedByNavigations)
                    .HasForeignKey(d => d.ReportedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Report__reported__236943A5");

                entity.HasOne(d => d.ReportedOnNavigation)
                    .WithMany(p => p.ReportReportedOnNavigations)
                    .HasForeignKey(d => d.ReportedOn)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Report__reported__245D67DE");
            });

            modelBuilder.Entity<Restaurant>(entity =>
            {
                entity.HasKey(e => e.Uid)
                    .HasName("PK__Restaura__DD701264ED48612C");

                entity.ToTable("Restaurant");

                entity.Property(e => e.Uid)
                    .ValueGeneratedNever()
                    .HasColumnName("uid");

                entity.Property(e => e.Address)
                    .HasMaxLength(255)
                    .HasColumnName("address");

                entity.Property(e => e.CancellationFeePercent)
                    .HasColumnType("decimal(5, 2)")
                    .HasColumnName("cancellation_fee_percent")
                    .HasDefaultValueSql("((100))");

                entity.Property(e => e.CategoryRestaurantId).HasColumnName("category_restaurant_id");

                entity.Property(e => e.CloseTime).HasColumnName("close_time");

                entity.Property(e => e.Commune)
                    .HasMaxLength(255)
                    .HasColumnName("commune");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.Discount)
                    .HasColumnType("decimal(5, 2)")
                    .HasColumnName("discount")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.District)
                    .HasMaxLength(255)
                    .HasColumnName("district");

                entity.Property(e => e.FirstFeePercent)
                    .HasColumnType("decimal(5, 2)")
                    .HasColumnName("first_fee_percent")
                    .HasDefaultValueSql("((100))");

                entity.Property(e => e.IsBookingEnabled)
                    .HasColumnName("is_booking_enabled")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Logo)
                    .HasMaxLength(255)
                    .HasColumnName("logo");

                entity.Property(e => e.MaxCapacity).HasColumnName("max_capacity");

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
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("price");

                entity.Property(e => e.ProvinceCity)
                    .HasMaxLength(255)
                    .HasColumnName("province_city");

                entity.Property(e => e.ReturningFeePercent)
                    .HasColumnType("decimal(5, 2)")
                    .HasColumnName("returning_fee_percent")
                    .HasDefaultValueSql("((100))");

                entity.Property(e => e.Subdescription).HasColumnName("subdescription");

                entity.HasOne(d => d.CategoryRestaurant)
                    .WithMany(p => p.Restaurants)
                    .HasForeignKey(d => d.CategoryRestaurantId)
                    .HasConstraintName("FK__Restauran__categ__45F365D3");

                entity.HasOne(d => d.UidNavigation)
                    .WithOne(p => p.Restaurant)
                    .HasForeignKey<Restaurant>(d => d.Uid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Restaurant__uid__46E78A0C");
            });

            modelBuilder.Entity<RestaurantRoom>(entity =>
            {
                entity.HasKey(e => e.RoomId)
                    .HasName("PK__Restaura__19675A8AC0B0BDE7");

                entity.ToTable("Restaurant_Room");

                entity.Property(e => e.RoomId).HasColumnName("room_id");

                entity.Property(e => e.CategoryRoomId).HasColumnName("category_room_id");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.IsBookingEnabled)
                    .HasColumnName("is_booking_enabled")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.MaxCapacity).HasColumnName("max_capacity");

                entity.Property(e => e.RestaurantId).HasColumnName("restaurant_id");

                entity.Property(e => e.RoomName)
                    .HasMaxLength(255)
                    .HasColumnName("room_name");

                entity.HasOne(d => d.CategoryRoom)
                    .WithMany(p => p.RestaurantRooms)
                    .HasForeignKey(d => d.CategoryRoomId)
                    .HasConstraintName("FK__Restauran__categ__4E88ABD4");

                entity.HasOne(d => d.Restaurant)
                    .WithMany(p => p.RestaurantRooms)
                    .HasForeignKey(d => d.RestaurantId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Restauran__resta__4D94879B");
            });

            modelBuilder.Entity<RestaurantTable>(entity =>
            {
                entity.HasKey(e => e.TableId)
                    .HasName("PK__Restaura__B21E8F24626B9A19");

                entity.ToTable("Restaurant_Table");

                entity.Property(e => e.TableId).HasColumnName("table_id");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.IsBookingEnabled)
                    .HasColumnName("is_booking_enabled")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.MaxCapacity).HasColumnName("max_capacity");

                entity.Property(e => e.RestaurantId).HasColumnName("restaurant_id");

                entity.Property(e => e.RoomId).HasColumnName("room_id");

                entity.Property(e => e.TableName)
                    .HasMaxLength(255)
                    .HasColumnName("table_name");

                entity.HasOne(d => d.Restaurant)
                    .WithMany(p => p.RestaurantTables)
                    .HasForeignKey(d => d.RestaurantId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Restauran__resta__52593CB8");

                entity.HasOne(d => d.Room)
                    .WithMany(p => p.RestaurantTables)
                    .HasForeignKey(d => d.RoomId)
                    .HasConstraintName("FK__Restauran__room___534D60F1");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role");

                entity.Property(e => e.RoleId).HasColumnName("role_id");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<TableBookingSchedule>(entity =>
            {
                entity.HasKey(e => e.ScheduleId)
                    .HasName("PK__Table_Bo__C46A8A6F51DAADAA");

                entity.ToTable("Table_Booking_Schedule");

                entity.Property(e => e.ScheduleId).HasColumnName("schedule_id");

                entity.Property(e => e.EndTime)
                    .HasColumnType("datetime")
                    .HasColumnName("end_time");

                entity.Property(e => e.Notes)
                    .HasMaxLength(255)
                    .HasColumnName("notes");

                entity.Property(e => e.RestaurantId).HasColumnName("restaurant_id");

                entity.Property(e => e.StartTime)
                    .HasColumnType("datetime")
                    .HasColumnName("start_time");

                entity.Property(e => e.TableId).HasColumnName("table_id");

                entity.HasOne(d => d.Restaurant)
                    .WithMany(p => p.TableBookingSchedules)
                    .HasForeignKey(d => d.RestaurantId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Table_Boo__resta__571DF1D5");

                entity.HasOne(d => d.Table)
                    .WithMany(p => p.TableBookingSchedules)
                    .HasForeignKey(d => d.TableId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Table_Boo__table__5629CD9C");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Uid)
                    .HasName("PK__User__DD70126424684674");

                entity.ToTable("User");

                entity.HasIndex(e => e.Email, "UQ__User__AB6E6164EAC8E0F7")
                    .IsUnique();

                entity.Property(e => e.Uid).HasColumnName("uid");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Email)
                    .HasMaxLength(255)
                    .HasColumnName("email");

                entity.Property(e => e.IsExternalLogin).HasColumnName("is_external_login");

                entity.Property(e => e.IsVerify).HasColumnName("is_verify");

                entity.Property(e => e.Password)
                    .HasMaxLength(255)
                    .HasColumnName("password");

                entity.Property(e => e.RoleId).HasColumnName("role_id");

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .HasColumnName("status");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__User__role_id__2A4B4B5E");
            });

            modelBuilder.Entity<UserOtp>(entity =>
            {
                entity.HasKey(e => e.OtpId)
                    .HasName("PK__User_OTP__AEE3543542ECA54B");

                entity.ToTable("User_OTP");

                entity.Property(e => e.OtpId).HasColumnName("otp_id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ExpiresAt)
                    .HasColumnType("datetime")
                    .HasColumnName("expires_at");

                entity.Property(e => e.IsUse)
                    .HasColumnName("is_use")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.OtpCode)
                    .HasMaxLength(6)
                    .HasColumnName("otp_code");

                entity.Property(e => e.Uid).HasColumnName("uid");

                entity.HasOne(d => d.UidNavigation)
                    .WithMany(p => p.UserOtps)
                    .HasForeignKey(d => d.Uid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__User_OTP__uid__2F10007B");
            });

            modelBuilder.Entity<Wallet>(entity =>
            {
                entity.ToTable("Wallet");

                entity.Property(e => e.WalletId).HasColumnName("wallet_id");

                entity.Property(e => e.AccountName)
                    .HasMaxLength(255)
                    .HasColumnName("account_name");

                entity.Property(e => e.AccountNo)
                    .HasMaxLength(50)
                    .HasColumnName("account_no");

                entity.Property(e => e.BankCode)
                    .HasMaxLength(50)
                    .HasColumnName("bank_code");

                entity.Property(e => e.OtpCode)
                    .HasMaxLength(10)
                    .HasColumnName("otp_code");

                entity.Property(e => e.Uid).HasColumnName("uid");

                entity.Property(e => e.WalletBalance)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("wallet_balance")
                    .HasDefaultValueSql("((0))");

                entity.HasOne(d => d.UidNavigation)
                    .WithMany(p => p.Wallets)
                    .HasForeignKey(d => d.Uid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Wallet__uid__5AEE82B9");
            });

            modelBuilder.Entity<WalletTransaction>(entity =>
            {
                entity.HasKey(e => e.TransactionId)
                    .HasName("PK__Wallet_T__85C600AF0A951D14");

                entity.ToTable("Wallet_Transaction");

                entity.Property(e => e.TransactionId).HasColumnName("transaction_id");

                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .HasColumnName("description");

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .HasColumnName("status");

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

                entity.Property(e => e.WalletId).HasColumnName("wallet_id");

                entity.HasOne(d => d.Wallet)
                    .WithMany(p => p.WalletTransactions)
                    .HasForeignKey(d => d.WalletId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Wallet_Tr__walle__5EBF139D");
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
                    .HasConstraintName("FK__Wishlist__custom__160F4887");

                entity.HasOne(d => d.Restaurant)
                    .WithMany(p => p.Wishlists)
                    .HasForeignKey(d => d.RestaurantId)
                    .HasConstraintName("FK__Wishlist__restau__17036CC0");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
