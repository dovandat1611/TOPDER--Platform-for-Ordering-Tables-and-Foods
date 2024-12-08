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
        public virtual DbSet<AdvertisementPricing> AdvertisementPricings { get; set; } = null!;
        public virtual DbSet<Blog> Blogs { get; set; } = null!;
        public virtual DbSet<BlogGroup> BlogGroups { get; set; } = null!;
        public virtual DbSet<BookingAdvertisement> BookingAdvertisements { get; set; } = null!;
        public virtual DbSet<CategoryMenu> CategoryMenus { get; set; } = null!;
        public virtual DbSet<CategoryRestaurant> CategoryRestaurants { get; set; } = null!;
        public virtual DbSet<Chat> Chats { get; set; } = null!;
        public virtual DbSet<ChatBox> ChatBoxes { get; set; } = null!;
        public virtual DbSet<Contact> Contacts { get; set; } = null!;
        public virtual DbSet<Customer> Customers { get; set; } = null!;
        public virtual DbSet<Discount> Discounts { get; set; } = null!;
        public virtual DbSet<DiscountMenu> DiscountMenus { get; set; } = null!;
        public virtual DbSet<ExternalLogin> ExternalLogins { get; set; } = null!;
        public virtual DbSet<Feedback> Feedbacks { get; set; } = null!;
        public virtual DbSet<FeedbackReply> FeedbackReplies { get; set; } = null!;
        public virtual DbSet<Image> Images { get; set; } = null!;
        public virtual DbSet<Menu> Menus { get; set; } = null!;
        public virtual DbSet<Notification> Notifications { get; set; } = null!;
        public virtual DbSet<Order> Orders { get; set; } = null!;
        public virtual DbSet<OrderMenu> OrderMenus { get; set; } = null!;
        public virtual DbSet<OrderTable> OrderTables { get; set; } = null!;
        public virtual DbSet<PolicySystem> PolicySystems { get; set; } = null!;
        public virtual DbSet<Report> Reports { get; set; } = null!;
        public virtual DbSet<Restaurant> Restaurants { get; set; } = null!;
        public virtual DbSet<RestaurantPolicy> RestaurantPolicies { get; set; } = null!;
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
                    .HasName("PK__Admin__DD7012647EE83E8C");

                entity.ToTable("Admin");

                entity.Property(e => e.Uid)
                    .ValueGeneratedNever()
                    .HasColumnName("uid");

                entity.Property(e => e.Dob)
                    .HasColumnType("date")
                    .HasColumnName("dob");

                entity.Property(e => e.Image).HasColumnName("image");

                entity.Property(e => e.Name).HasColumnName("name");

                entity.Property(e => e.Phone).HasColumnName("phone");

                entity.HasOne(d => d.UidNavigation)
                    .WithOne(p => p.Admin)
                    .HasForeignKey<Admin>(d => d.Uid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Admin__uid__36B12243");
            });

            modelBuilder.Entity<AdvertisementPricing>(entity =>
            {
                entity.HasKey(e => e.PricingId)
                    .HasName("PK__Advertis__A25A9FB7B02B61E6");

                entity.ToTable("AdvertisementPricing");

                entity.Property(e => e.PricingId).HasColumnName("pricing_id");

                entity.Property(e => e.AdminId).HasColumnName("admin_id");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.DurationHours).HasColumnName("duration_hours");

                entity.Property(e => e.Price)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("price");

                entity.Property(e => e.PricingName).HasColumnName("pricing_name");

                entity.HasOne(d => d.Admin)
                    .WithMany(p => p.AdvertisementPricings)
                    .HasForeignKey(d => d.AdminId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Advertise__admin__3E1D39E1");
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

                entity.Property(e => e.Image).HasColumnName("image");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.Title).HasColumnName("title");

                entity.HasOne(d => d.Admin)
                    .WithMany(p => p.Blogs)
                    .HasForeignKey(d => d.AdminId)
                    .HasConstraintName("FK__Blog__admin_id__74AE54BC");

                entity.HasOne(d => d.Bloggroup)
                    .WithMany(p => p.Blogs)
                    .HasForeignKey(d => d.BloggroupId)
                    .HasConstraintName("FK__Blog__bloggroup___73BA3083");
            });

            modelBuilder.Entity<BlogGroup>(entity =>
            {
                entity.ToTable("Blog_Group");

                entity.Property(e => e.BloggroupId).HasColumnName("bloggroup_id");

                entity.Property(e => e.BloggroupName).HasColumnName("bloggroup_name");
            });

            modelBuilder.Entity<BookingAdvertisement>(entity =>
            {
                entity.HasKey(e => e.BookingId)
                    .HasName("PK__BookingA__5DE3A5B1AB52CFA0");

                entity.ToTable("BookingAdvertisement");

                entity.Property(e => e.BookingId).HasColumnName("booking_id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.EndTime)
                    .HasColumnType("datetime")
                    .HasColumnName("end_time");

                entity.Property(e => e.RestaurantId).HasColumnName("restaurant_id");

                entity.Property(e => e.StartTime)
                    .HasColumnType("datetime")
                    .HasColumnName("start_time");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasDefaultValueSql("('Active')");

                entity.Property(e => e.StatusPayment).HasColumnName("status_payment");

                entity.Property(e => e.Title).HasColumnName("title");

                entity.Property(e => e.TotalAmount)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("total_amount");

                entity.HasOne(d => d.Restaurant)
                    .WithMany(p => p.BookingAdvertisements)
                    .HasForeignKey(d => d.RestaurantId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__BookingAd__resta__3B40CD36");
            });

            modelBuilder.Entity<CategoryMenu>(entity =>
            {
                entity.ToTable("Category_Menu");

                entity.Property(e => e.CategoryMenuId).HasColumnName("category_menu_id");

                entity.Property(e => e.CategoryMenuName).HasColumnName("category_menu_name");

                entity.Property(e => e.IsVisible)
                    .HasColumnName("is_visible")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.RestaurantId).HasColumnName("restaurant_id");

                entity.HasOne(d => d.Restaurant)
                    .WithMany(p => p.CategoryMenus)
                    .HasForeignKey(d => d.RestaurantId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Category___resta__6754599E");
            });

            modelBuilder.Entity<CategoryRestaurant>(entity =>
            {
                entity.ToTable("Category_Restaurant");

                entity.Property(e => e.CategoryRestaurantId).HasColumnName("category_restaurant_id");

                entity.Property(e => e.CategoryRestaurantName).HasColumnName("category_restaurant_name");
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
                    .HasConstraintName("FK__Chat__chat_box_i__2A164134");

                entity.HasOne(d => d.ChatByNavigation)
                    .WithMany(p => p.Chats)
                    .HasForeignKey(d => d.ChatBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Chat__chat_by__2B0A656D");
            });

            modelBuilder.Entity<ChatBox>(entity =>
            {
                entity.ToTable("ChatBox");

                entity.Property(e => e.ChatBoxId).HasColumnName("chat_box_id");

                entity.Property(e => e.CustomerId).HasColumnName("customer_id");

                entity.Property(e => e.IsCustomerRead)
                    .HasColumnName("is_customer_read")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsRestaurantRead)
                    .HasColumnName("is_restaurant_read")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.RestaurantId).HasColumnName("restaurant_id");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.ChatBoxes)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ChatBox__custome__25518C17");

                entity.HasOne(d => d.Restaurant)
                    .WithMany(p => p.ChatBoxes)
                    .HasForeignKey(d => d.RestaurantId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ChatBox__restaur__2645B050");
            });

            modelBuilder.Entity<Contact>(entity =>
            {
                entity.ToTable("Contact");

                entity.Property(e => e.ContactId).HasColumnName("contact_id");

                entity.Property(e => e.ContactDate)
                    .HasColumnType("date")
                    .HasColumnName("contact_date");

                entity.Property(e => e.Content).HasColumnName("content");

                entity.Property(e => e.Email).HasColumnName("email");

                entity.Property(e => e.Name).HasColumnName("name");

                entity.Property(e => e.Phone).HasColumnName("phone");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.Topic).HasColumnName("topic");

                entity.Property(e => e.Uid).HasColumnName("uid");

                entity.HasOne(d => d.UidNavigation)
                    .WithMany(p => p.Contacts)
                    .HasForeignKey(d => d.Uid)
                    .HasConstraintName("FK__Contact__uid__778AC167");
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.Uid)
                    .HasName("PK__Customer__DD701264F28E4C38");

                entity.ToTable("Customer");

                entity.Property(e => e.Uid)
                    .ValueGeneratedNever()
                    .HasColumnName("uid");

                entity.Property(e => e.Dob)
                    .HasColumnType("date")
                    .HasColumnName("dob");

                entity.Property(e => e.Gender).HasColumnName("gender");

                entity.Property(e => e.Image).HasColumnName("image");

                entity.Property(e => e.Name).HasColumnName("name");

                entity.Property(e => e.Phone).HasColumnName("phone");

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

                entity.Property(e => e.ApplicableTo).HasColumnName("applicable_to");

                entity.Property(e => e.ApplyType).HasColumnName("apply_type");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.DiscountName).HasColumnName("discount_name");

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

                entity.Property(e => e.IsVisible)
                    .HasColumnName("is_visible")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.MaxOrderValue)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("max_order_value");

                entity.Property(e => e.MinOrderValue)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("min_order_value");

                entity.Property(e => e.Quantity).HasColumnName("quantity");

                entity.Property(e => e.RestaurantId).HasColumnName("restaurant_id");

                entity.Property(e => e.Scope).HasColumnName("scope");

                entity.Property(e => e.StartDate)
                    .HasColumnType("datetime")
                    .HasColumnName("start_date");

                entity.HasOne(d => d.Restaurant)
                    .WithMany(p => p.Discounts)
                    .HasForeignKey(d => d.RestaurantId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Discount__restau__7E37BEF6");
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
                    .HasConstraintName("FK__Discount___disco__02084FDA");

                entity.HasOne(d => d.Menu)
                    .WithMany(p => p.DiscountMenus)
                    .HasForeignKey(d => d.MenuId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Discount___menu___02FC7413");
            });

            modelBuilder.Entity<ExternalLogin>(entity =>
            {
                entity.ToTable("External_Logins");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AccessToken).HasColumnName("access_token");

                entity.Property(e => e.ExternalProvider).HasColumnName("external_provider");

                entity.Property(e => e.ExternalUserId).HasColumnName("external_user_id");

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

                entity.Property(e => e.IsVisible)
                    .HasColumnName("is_visible")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.OrderId).HasColumnName("order_id");

                entity.Property(e => e.RestaurantId).HasColumnName("restaurant_id");

                entity.Property(e => e.Star).HasColumnName("star");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Feedbacks)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK__Feedback__custom__160F4887");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.Feedbacks)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Feedback__order___151B244E");

                entity.HasOne(d => d.Restaurant)
                    .WithMany(p => p.Feedbacks)
                    .HasForeignKey(d => d.RestaurantId)
                    .HasConstraintName("FK__Feedback__restau__17036CC0");
            });

            modelBuilder.Entity<FeedbackReply>(entity =>
            {
                entity.HasKey(e => e.ReplyId)
                    .HasName("PK__Feedback__EE40569815BE94A7");

                entity.ToTable("FeedbackReply");

                entity.Property(e => e.ReplyId).HasColumnName("reply_id");

                entity.Property(e => e.Content).HasColumnName("content");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("create_date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FeedbackId).HasColumnName("feedback_id");

                entity.Property(e => e.IsVisible)
                    .HasColumnName("is_visible")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.RestaurantId).HasColumnName("restaurant_id");

                entity.HasOne(d => d.Feedback)
                    .WithMany(p => p.FeedbackReplies)
                    .HasForeignKey(d => d.FeedbackId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__FeedbackR__feedb__1BC821DD");

                entity.HasOne(d => d.Restaurant)
                    .WithMany(p => p.FeedbackReplies)
                    .HasForeignKey(d => d.RestaurantId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__FeedbackR__resta__1CBC4616");
            });

            modelBuilder.Entity<Image>(entity =>
            {
                entity.ToTable("Image");

                entity.Property(e => e.ImageId).HasColumnName("image_id");

                entity.Property(e => e.ImageUrl).HasColumnName("imageUrl");

                entity.Property(e => e.RestaurantId).HasColumnName("restaurant_id");

                entity.HasOne(d => d.Restaurant)
                    .WithMany(p => p.Images)
                    .HasForeignKey(d => d.RestaurantId)
                    .HasConstraintName("FK__Image__restauran__6EF57B66");
            });

            modelBuilder.Entity<Menu>(entity =>
            {
                entity.ToTable("Menu");

                entity.Property(e => e.MenuId).HasColumnName("menu_id");

                entity.Property(e => e.CategoryMenuId).HasColumnName("category_menu_id");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.DishName).HasColumnName("dish_name");

                entity.Property(e => e.Image).HasColumnName("image");

                entity.Property(e => e.IsVisible)
                    .HasColumnName("is_visible")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Price)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("price");

                entity.Property(e => e.RestaurantId).HasColumnName("restaurant_id");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.HasOne(d => d.CategoryMenu)
                    .WithMany(p => p.Menus)
                    .HasForeignKey(d => d.CategoryMenuId)
                    .HasConstraintName("FK__Menu__category_m__6B24EA82");

                entity.HasOne(d => d.Restaurant)
                    .WithMany(p => p.Menus)
                    .HasForeignKey(d => d.RestaurantId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Menu__restaurant__6C190EBB");
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

                entity.Property(e => e.Type).HasColumnName("type");

                entity.Property(e => e.Uid).HasColumnName("uid");

                entity.HasOne(d => d.UidNavigation)
                    .WithMany(p => p.Notifications)
                    .HasForeignKey(d => d.Uid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Notificatio__uid__367C1819");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Order");

                entity.Property(e => e.OrderId).HasColumnName("order_id");

                entity.Property(e => e.CancelReason).HasColumnName("cancel_reason");

                entity.Property(e => e.CancelledAt)
                    .HasColumnType("datetime")
                    .HasColumnName("cancelled_at");

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

                entity.Property(e => e.DepositAmount)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("deposit_amount");

                entity.Property(e => e.DiscountId).HasColumnName("discount_id");

                entity.Property(e => e.FoodAddAmount)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("foodAdd_amount");

                entity.Property(e => e.FoodAmount)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("food_amount");

                entity.Property(e => e.NameReceiver).HasColumnName("name_receiver");

                entity.Property(e => e.NumberChild).HasColumnName("number_child");

                entity.Property(e => e.NumberPerson).HasColumnName("number_person");

                entity.Property(e => e.PaidAt)
                    .HasColumnType("datetime")
                    .HasColumnName("paid_at");

                entity.Property(e => e.PaidType).HasColumnName("paid_type");

                entity.Property(e => e.PhoneReceiver).HasColumnName("phone_receiver");

                entity.Property(e => e.RestaurantId).HasColumnName("restaurant_id");

                entity.Property(e => e.StatusOrder).HasColumnName("status_order");

                entity.Property(e => e.StatusPayment).HasColumnName("status_payment");

                entity.Property(e => e.TimeReservation).HasColumnName("time_reservation");

                entity.Property(e => e.TotalAmount)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("total_amount");

                entity.Property(e => e.TypeOrder).HasColumnName("type_order");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK__Order__customer___05D8E0BE");

                entity.HasOne(d => d.Discount)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.DiscountId)
                    .HasConstraintName("FK__Order__discount___07C12930");

                entity.HasOne(d => d.Restaurant)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.RestaurantId)
                    .HasConstraintName("FK__Order__restauran__06CD04F7");
            });

            modelBuilder.Entity<OrderMenu>(entity =>
            {
                entity.ToTable("Order_Menu");

                entity.Property(e => e.OrderMenuId).HasColumnName("order_menu_id");

                entity.Property(e => e.MenuId).HasColumnName("menu_id");

                entity.Property(e => e.OrderId).HasColumnName("order_id");

                entity.Property(e => e.OrderMenuType).HasColumnName("order_menu_type");

                entity.Property(e => e.Price)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("price");

                entity.Property(e => e.Quantity).HasColumnName("quantity");

                entity.HasOne(d => d.Menu)
                    .WithMany(p => p.OrderMenus)
                    .HasForeignKey(d => d.MenuId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Order_Men__menu___0F624AF8");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderMenus)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Order_Men__order__0E6E26BF");
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
                    .HasConstraintName("FK__Order_Tab__order__0A9D95DB");

                entity.HasOne(d => d.Table)
                    .WithMany(p => p.OrderTables)
                    .HasForeignKey(d => d.TableId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Order_Tab__table__0B91BA14");
            });

            modelBuilder.Entity<PolicySystem>(entity =>
            {
                entity.HasKey(e => e.PolicyId)
                    .HasName("PK__Policy_S__47DA3F03B905B10B");

                entity.ToTable("Policy_System");

                entity.Property(e => e.PolicyId).HasColumnName("policy_id");

                entity.Property(e => e.AdminId).HasColumnName("admin_id");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("create_date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FeeAmount)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("fee_amount");

                entity.Property(e => e.MaxOrderValue)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("max_order_value");

                entity.Property(e => e.MinOrderValue)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("min_order_value");

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .HasColumnName("status")
                    .HasDefaultValueSql("('Active')");

                entity.HasOne(d => d.Admin)
                    .WithMany(p => p.PolicySystems)
                    .HasForeignKey(d => d.AdminId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Policy_Sy__admin__45BE5BA9");
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

                entity.Property(e => e.FeedbackId).HasColumnName("feedback_id");

                entity.Property(e => e.OrderId).HasColumnName("order_id");

                entity.Property(e => e.ReportType).HasColumnName("report_type");

                entity.Property(e => e.ReportedBy).HasColumnName("reported_by");

                entity.Property(e => e.ReportedOn).HasColumnName("reported_on");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.HasOne(d => d.Feedback)
                    .WithMany(p => p.Reports)
                    .HasForeignKey(d => d.FeedbackId)
                    .HasConstraintName("FK__Report__feedback__30C33EC3");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.Reports)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("FK__Report__order_id__31B762FC");

                entity.HasOne(d => d.ReportedByNavigation)
                    .WithMany(p => p.ReportReportedByNavigations)
                    .HasForeignKey(d => d.ReportedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Report__reported__2EDAF651");

                entity.HasOne(d => d.ReportedOnNavigation)
                    .WithMany(p => p.ReportReportedOnNavigations)
                    .HasForeignKey(d => d.ReportedOn)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Report__reported__2FCF1A8A");
            });

            modelBuilder.Entity<Restaurant>(entity =>
            {
                entity.HasKey(e => e.Uid)
                    .HasName("PK__Restaura__DD70126454852A88");

                entity.ToTable("Restaurant");

                entity.Property(e => e.Uid)
                    .ValueGeneratedNever()
                    .HasColumnName("uid");

                entity.Property(e => e.Address).HasColumnName("address");

                entity.Property(e => e.CategoryRestaurantId).HasColumnName("category_restaurant_id");

                entity.Property(e => e.CloseTime).HasColumnName("close_time");

                entity.Property(e => e.Commune).HasColumnName("commune");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.Discount)
                    .HasColumnType("decimal(5, 2)")
                    .HasColumnName("discount")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.District).HasColumnName("district");

                entity.Property(e => e.IsBookingEnabled)
                    .HasColumnName("is_booking_enabled")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Logo).HasColumnName("logo");

                entity.Property(e => e.MaxCapacity).HasColumnName("max_capacity");

                entity.Property(e => e.NameOwner).HasColumnName("name_owner");

                entity.Property(e => e.NameRes).HasColumnName("name_res");

                entity.Property(e => e.OpenTime).HasColumnName("open_time");

                entity.Property(e => e.Phone).HasColumnName("phone");

                entity.Property(e => e.Price)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("price");

                entity.Property(e => e.ProvinceCity).HasColumnName("province_city");

                entity.Property(e => e.ReputationScore)
                    .HasColumnName("reputation_score")
                    .HasDefaultValueSql("((100))");

                entity.Property(e => e.Subdescription).HasColumnName("subdescription");

                entity.Property(e => e.TableGapTime)
                    .HasColumnName("table_gap_time")
                    .HasDefaultValueSql("((1))");

                entity.HasOne(d => d.CategoryRestaurant)
                    .WithMany(p => p.Restaurants)
                    .HasForeignKey(d => d.CategoryRestaurantId)
                    .HasConstraintName("FK__Restauran__categ__4222D4EF");

                entity.HasOne(d => d.UidNavigation)
                    .WithOne(p => p.Restaurant)
                    .HasForeignKey<Restaurant>(d => d.Uid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Restaurant__uid__4316F928");
            });

            modelBuilder.Entity<RestaurantPolicy>(entity =>
            {
                entity.HasKey(e => e.PolicyId)
                    .HasName("PK__Restaura__47DA3F03AD1BC115");

                entity.ToTable("Restaurant_Policy");

                entity.Property(e => e.PolicyId).HasColumnName("policy_id");

                entity.Property(e => e.CancellationFeePercent)
                    .HasColumnType("decimal(5, 2)")
                    .HasColumnName("cancellation_fee_percent")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("create_date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FirstFeePercent)
                    .HasColumnType("decimal(5, 2)")
                    .HasColumnName("first_fee_percent")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.RestaurantId).HasColumnName("restaurant_id");

                entity.Property(e => e.ReturningFeePercent)
                    .HasColumnType("decimal(5, 2)")
                    .HasColumnName("returning_fee_percent")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .HasColumnName("status")
                    .HasDefaultValueSql("('Active')");

                entity.HasOne(d => d.Restaurant)
                    .WithMany(p => p.RestaurantPolicies)
                    .HasForeignKey(d => d.RestaurantId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Restauran__resta__4D94879B");
            });

            modelBuilder.Entity<RestaurantRoom>(entity =>
            {
                entity.HasKey(e => e.RoomId)
                    .HasName("PK__Restaura__19675A8A18311FEF");

                entity.ToTable("Restaurant_Room");

                entity.Property(e => e.RoomId).HasColumnName("room_id");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.IsBookingEnabled)
                    .HasColumnName("is_booking_enabled")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsVisible)
                    .HasColumnName("is_visible")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.MaxCapacity).HasColumnName("max_capacity");

                entity.Property(e => e.RestaurantId).HasColumnName("restaurant_id");

                entity.Property(e => e.RoomName).HasColumnName("room_name");

                entity.HasOne(d => d.Restaurant)
                    .WithMany(p => p.RestaurantRooms)
                    .HasForeignKey(d => d.RestaurantId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Restauran__resta__52593CB8");
            });

            modelBuilder.Entity<RestaurantTable>(entity =>
            {
                entity.HasKey(e => e.TableId)
                    .HasName("PK__Restaura__B21E8F24A274FF79");

                entity.ToTable("Restaurant_Table");

                entity.Property(e => e.TableId).HasColumnName("table_id");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.IsBookingEnabled)
                    .HasColumnName("is_booking_enabled")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsVisible)
                    .HasColumnName("is_visible")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.MaxCapacity).HasColumnName("max_capacity");

                entity.Property(e => e.RestaurantId).HasColumnName("restaurant_id");

                entity.Property(e => e.RoomId).HasColumnName("room_id");

                entity.Property(e => e.TableName).HasColumnName("table_name");

                entity.HasOne(d => d.Restaurant)
                    .WithMany(p => p.RestaurantTables)
                    .HasForeignKey(d => d.RestaurantId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Restauran__resta__571DF1D5");

                entity.HasOne(d => d.Room)
                    .WithMany(p => p.RestaurantTables)
                    .HasForeignKey(d => d.RoomId)
                    .HasConstraintName("FK__Restauran__room___5812160E");
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
                    .HasName("PK__Table_Bo__C46A8A6F7A4AB538");

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
                    .HasConstraintName("FK__Table_Boo__resta__5BE2A6F2");

                entity.HasOne(d => d.Table)
                    .WithMany(p => p.TableBookingSchedules)
                    .HasForeignKey(d => d.TableId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Table_Boo__table__5AEE82B9");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Uid)
                    .HasName("PK__User__DD70126441EA7465");

                entity.ToTable("User");

                entity.HasIndex(e => e.Email, "UQ__User__AB6E616443BE0410")
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

                entity.Property(e => e.Password).HasColumnName("password");

                entity.Property(e => e.RoleId).HasColumnName("role_id");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__User__role_id__2A4B4B5E");
            });

            modelBuilder.Entity<UserOtp>(entity =>
            {
                entity.HasKey(e => e.OtpId)
                    .HasName("PK__User_OTP__AEE354353F6D91F6");

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

                entity.Property(e => e.OtpCode).HasColumnName("otp_code");

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

                entity.Property(e => e.AccountName).HasColumnName("account_name");

                entity.Property(e => e.AccountNo).HasColumnName("account_no");

                entity.Property(e => e.BankCode).HasColumnName("bank_code");

                entity.Property(e => e.OtpCode).HasColumnName("otp_code");

                entity.Property(e => e.Uid).HasColumnName("uid");

                entity.Property(e => e.WalletBalance)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("wallet_balance")
                    .HasDefaultValueSql("((0))");

                entity.HasOne(d => d.UidNavigation)
                    .WithMany(p => p.Wallets)
                    .HasForeignKey(d => d.Uid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Wallet__uid__5FB337D6");
            });

            modelBuilder.Entity<WalletTransaction>(entity =>
            {
                entity.HasKey(e => e.TransactionId)
                    .HasName("PK__Wallet_T__85C600AF468802A2");

                entity.ToTable("Wallet_Transaction");

                entity.Property(e => e.TransactionId).HasColumnName("transaction_id");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.TransactionAmount)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("transaction_amount");

                entity.Property(e => e.TransactionDate)
                    .HasColumnType("datetime")
                    .HasColumnName("transaction_date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.TransactionType).HasColumnName("transaction_type");

                entity.Property(e => e.WalletId).HasColumnName("wallet_id");

                entity.HasOne(d => d.Wallet)
                    .WithMany(p => p.WalletTransactions)
                    .HasForeignKey(d => d.WalletId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Wallet_Tr__walle__6383C8BA");
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
                    .HasConstraintName("FK__Wishlist__custom__1F98B2C1");

                entity.HasOne(d => d.Restaurant)
                    .WithMany(p => p.Wishlists)
                    .HasForeignKey(d => d.RestaurantId)
                    .HasConstraintName("FK__Wishlist__restau__208CD6FA");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
