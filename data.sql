USE [TopderDB]
GO
SET IDENTITY_INSERT [dbo].[Role] ON 

INSERT [dbo].[Role] ([role_id], [name]) VALUES (1, N'Admin')
INSERT [dbo].[Role] ([role_id], [name]) VALUES (2, N'Restaurant')
INSERT [dbo].[Role] ([role_id], [name]) VALUES (3, N'Customer')
SET IDENTITY_INSERT [dbo].[Role] OFF
GO
SET IDENTITY_INSERT [dbo].[User] ON 

INSERT [dbo].[User] ([uid], [role_id], [email], [password],  [is_verify], [status], [created_at], [is_external_login]) VALUES (1, 1, N'topder.vn@gmail.com', N'$2a$11$bCy3gf5Qca1awsszGPYdke3I9bIvNCNWVtwRxHfcclrOluakbsKNK', 1, N'Active', CAST(N'2024-10-15T13:54:18.213' AS DateTime), 0)
INSERT [dbo].[User] ([uid], [role_id], [email], [password],  [is_verify], [status], [created_at], [is_external_login]) VALUES (2, 3, N'datdvhe161664@fpt.edu.vn', N'$2a$11$SSAKsdpn7NRGc8cZCjcM1ugRG1CmiEk4ZMvNbgzmS6OVu5C5LPSwC', 1, N'Active', CAST(N'2024-10-15T13:56:01.427' AS DateTime), 0)
INSERT [dbo].[User] ([uid], [role_id], [email], [password], [is_verify], [status], [created_at], [is_external_login]) VALUES (3, 2, N'dovandat1611@gmail.com', N'$2a$11$yK3dvHfZQjMST23LdKyDjeGdSwaT8qmj3YcrwxzKMCw0HUFIl9xNS', 1, N'In-Active', CAST(N'2024-10-15T13:58:38.483' AS DateTime), 0)
SET IDENTITY_INSERT [dbo].[User] OFF
GO
INSERT [dbo].[Customer] ([uid], [name], [phone], [image], [dob], [gender]) VALUES (2, N'Đỗ Văn Đạt', N'0902121881', N'https://res.cloudinary.com/do9iyczi3/image/upload/v1728023034/default-avatar-profile-icon_ecq8w3.jpg', CAST(N'2002-11-16' AS Date), N'Male')
GO
SET IDENTITY_INSERT [dbo].[Category_Restaurant] ON 

INSERT [dbo].[Category_Restaurant] ([category_restaurant_id], [category_restaurant_name]) VALUES (1, N'Đồ Ăn Nhanh')
INSERT [dbo].[Category_Restaurant] ([category_restaurant_id], [category_restaurant_name]) VALUES (2, N'Hải Sản')
INSERT [dbo].[Category_Restaurant] ([category_restaurant_id], [category_restaurant_name]) VALUES (3, N'Buffet')
INSERT [dbo].[Category_Restaurant] ([category_restaurant_id], [category_restaurant_name]) VALUES (4, N'Quán Nhậu')
INSERT [dbo].[Category_Restaurant] ([category_restaurant_id], [category_restaurant_name]) VALUES (5, N'Quán Đồ Hàn')
INSERT [dbo].[Category_Restaurant] ([category_restaurant_id], [category_restaurant_name]) VALUES (6, N'Quán Nước')
INSERT [dbo].[Category_Restaurant] ([category_restaurant_id], [category_restaurant_name]) VALUES (7, N'Quán Đồ Nhật')
SET IDENTITY_INSERT [dbo].[Category_Restaurant] OFF
GO
INSERT [dbo].[Restaurant] ([uid], [category_restaurant_id], [name_owner], [name_res], [logo], [open_time], [close_time], [address], [phone], [description], [subdescription], [province_city],[district],[commune], [discount], [max_capacity], [price], [is_booking_enabled], [first_fee_percent], [returning_fee_percent], [cancellation_fee_percent]) VALUES (3, 7, N'Đỗ Văn Đạt', N'Mer. coffee & tea ', N'https://res.cloudinary.com/do9iyczi3/image/upload/v1728975519/hesdfxvomskasmtj0zea.jpg', CAST(N'08:00:00' AS Time), CAST(N'23:00:00' AS Time), N'A3-15 KDG Cổng Chung, Tân Hội, Đan Phượng, Hà Nội', N'0968519615', NULL, NULL, N'01', N'276', N'10000', CAST(0.00 AS Decimal(5, 2)), 30, CAST(20000.00 AS Decimal(18, 2)), 1, CAST(100.00 AS Decimal(5, 2)), CAST(100.00 AS Decimal(5, 2)), CAST(100.00 AS Decimal(5, 2)))
GO
INSERT [dbo].[Admin] ([uid], [name], [phone], [dob], [image]) VALUES (1, N'TOPDER', N'0968519615', CAST(N'2024-10-15' AS Date), N'https://res.cloudinary.com/do9iyczi3/image/upload/v1726643328/LOGO-TOPDER_qonl9l.png')
GO
SET IDENTITY_INSERT [dbo].[Wallet] ON 

INSERT [dbo].[Wallet] ([wallet_id], [uid], [wallet_balance], [bank_code], [account_no], [account_name], [otp_code]) VALUES (1, 1, CAST(0.00 AS Decimal(18, 2)), NULL, NULL, NULL, NULL)
INSERT [dbo].[Wallet] ([wallet_id], [uid], [wallet_balance], [bank_code], [account_no], [account_name], [otp_code]) VALUES (2, 2, CAST(0.00 AS Decimal(18, 2)), NULL, NULL, NULL, NULL)
INSERT [dbo].[Wallet] ([wallet_id], [uid], [wallet_balance], [bank_code], [account_no], [account_name], [otp_code]) VALUES (3, 3, CAST(0.00 AS Decimal(18, 2)), NULL, NULL, NULL, NULL)
SET IDENTITY_INSERT [dbo].[Wallet] OFF
GO

SET IDENTITY_INSERT [dbo].[Blog_Group] ON

INSERT [dbo].[Blog_Group] ([bloggroup_id], [bloggroup_name]) VALUES (1, N'Hướng Dẫn')
INSERT [dbo].[Blog_Group] ([bloggroup_id], [bloggroup_name]) VALUES (2, N'Review')
INSERT [dbo].[Blog_Group] ([bloggroup_id], [bloggroup_name]) VALUES (3, N'Sự Kiện')
INSERT [dbo].[Blog_Group] ([bloggroup_id], [bloggroup_name]) VALUES (4, N'Công Thức Nấu Ăn')
INSERT [dbo].[Blog_Group] ([bloggroup_id], [bloggroup_name]) VALUES (5, N'Ẩm Thực Địa Phương')
INSERT [dbo].[Blog_Group] ([bloggroup_id], [bloggroup_name]) VALUES (6, N'Tin Tức Ẩm Thực')

SET IDENTITY_INSERT [dbo].[Blog_Group] OFF 

GO

SET IDENTITY_INSERT [dbo].[Blog] ON 

INSERT [dbo].[Blog] ([blog_id], [bloggroup_id], [admin_id], [image], [title], [content], [create_date], [status]) VALUES (1, 1, 1, N'https://kenh14cdn.com/thumb_w/640/203336854389633024/2022/1/27/photo1643280875965-1643280876272838857615.png', N'Hướng Dẫn Nấu Ăn Dễ Dàng', N'Bài viết này cung cấp hướng dẫn chi tiết để nấu các món ăn đơn giản tại nhà.', CAST(N'2024-09-01' AS Date), N'Active')
INSERT [dbo].[Blog] ([blog_id], [bloggroup_id], [admin_id], [image], [title], [content], [create_date], [status]) VALUES (2, 2, 1, N'https://acecookvietnam.vn/wp-content/uploads/2020/08/1.jpg', N'Nhà Hàng Tốt Nhất Năm 2024', N'Khám phá những nhà hàng hàng đầu trên toàn thế giới.', CAST(N'2024-09-02' AS Date), N'In-Active')
INSERT [dbo].[Blog] ([blog_id], [bloggroup_id], [admin_id], [image], [title], [content], [create_date], [status]) VALUES (3, 3, 1, N'https://acecookvietnam.vn/wp-content/uploads/2020/08/6.jpg', N'Sự Kiện Ẩm Thực Năm 2024', N'Tìm hiểu về những sự kiện ẩm thực nổi bật trong năm tới.', CAST(N'2024-09-03' AS Date), N'Active')
INSERT [dbo].[Blog] ([blog_id], [bloggroup_id], [admin_id], [image], [title], [content], [create_date], [status]) VALUES (4, 4, 1, N'https://kenh14cdn.com/thumb_w/660/203336854389633024/2022/1/27/5-canh-bong-tha-1643280748691912438132.png', N'Công Thức Nấu Các Món Ăn Ngon', N'Cung cấp công thức nấu ăn cho các món ăn ngon và hấp dẫn.', CAST(N'2024-09-04' AS Date), N'Active')
INSERT [dbo].[Blog] ([blog_id], [bloggroup_id], [admin_id], [image], [title], [content], [create_date], [status]) VALUES (5, 5, 1, N'https://kenh14cdn.com/thumb_w/660/203336854389633024/2022/1/27/6-long-phuong-164328081849796407664.png', N'Khám Phá Ẩm Thực Địa Phương', N'Khám phá những món ăn đặc sản của từng vùng miền.', CAST(N'2024-09-05' AS Date), N'In-Active')

SET IDENTITY_INSERT [dbo].[Blog] OFF

GO 

SET IDENTITY_INSERT [dbo].[Category_Room] ON 

INSERT [dbo].[Category_Room] ([category_room_id], [category_name], [restaurant_id]) VALUES (1, N'Phòng VIP', 3)
INSERT [dbo].[Category_Room] ([category_room_id], [category_name], [restaurant_id]) VALUES (2, N'Phòng bình thường', 3)

SET IDENTITY_INSERT [dbo].[Category_Room] OFF
GO

SET IDENTITY_INSERT [dbo].[Restaurant_Room] ON 

INSERT [dbo].[Restaurant_Room] ([room_id], [restaurant_id], [category_room_id], [room_name], [max_capacity], [description], [is_booking_enabled]) VALUES (1, 3, 1, N'Phòng A', 10, N'Phòng có view biển A', 1)
INSERT [dbo].[Restaurant_Room] ([room_id], [restaurant_id], [category_room_id], [room_name], [max_capacity], [description], [is_booking_enabled]) VALUES (2, 3, 1, N'Phòng B', 8, N'Phòng VIP B', 1)
INSERT [dbo].[Restaurant_Room] ([room_id], [restaurant_id], [category_room_id], [room_name], [max_capacity], [description], [is_booking_enabled]) VALUES (3, 3, 2, N'Phòng C', 6, N'Phòng bình thường C', 0)
INSERT [dbo].[Restaurant_Room] ([room_id], [restaurant_id], [category_room_id], [room_name], [max_capacity], [description], [is_booking_enabled]) VALUES (4, 3, 2, N'Phòng D', 12, N'Phòng bình thường D', 1)
INSERT [dbo].[Restaurant_Room] ([room_id], [restaurant_id], [category_room_id], [room_name], [max_capacity], [description], [is_booking_enabled]) VALUES (5, 3, 1, N'Phòng A', 10, N'Phòng có view biển A', 1)
INSERT [dbo].[Restaurant_Room] ([room_id], [restaurant_id], [category_room_id], [room_name], [max_capacity], [description], [is_booking_enabled]) VALUES (6, 3, 1, N'Phòng B', 8, N'Phòng VIP B', 1)
INSERT [dbo].[Restaurant_Room] ([room_id], [restaurant_id], [category_room_id], [room_name], [max_capacity], [description], [is_booking_enabled]) VALUES (7, 3, 2, N'Phòng C', 6, N'Phòng bình thường C', 0)
INSERT [dbo].[Restaurant_Room] ([room_id], [restaurant_id], [category_room_id], [room_name], [max_capacity], [description], [is_booking_enabled]) VALUES (8, 3, 2, N'Phòng D', 12, N'Phòng bình thường D', 1)
SET IDENTITY_INSERT [dbo].[Restaurant_Room] OFF

GO

SET IDENTITY_INSERT [dbo].[Restaurant_Table] ON 

INSERT [dbo].[Restaurant_Table] ([table_id], [restaurant_id], [room_id], [table_name], [max_capacity], [description], [is_booking_enabled]) VALUES 
(1, 3, 1, N'Table 1A', 4, N'Bàn gần cửa sổ', 1)
INSERT [dbo].[Restaurant_Table] ([table_id], [restaurant_id], [room_id], [table_name], [max_capacity], [description], [is_booking_enabled]) VALUES 
(2, 3, 1, N'Table 2A', 6, N'Bàn lớn trong Phòng A', 1)
INSERT [dbo].[Restaurant_Table] ([table_id], [restaurant_id], [room_id], [table_name], [max_capacity], [description], [is_booking_enabled]) VALUES 
(3, 3, 2, N'Table 1B', 8, N'Bàn chính trong Phòng B', 1)
INSERT [dbo].[Restaurant_Table] ([table_id], [restaurant_id], [room_id], [table_name], [max_capacity], [description], [is_booking_enabled]) VALUES 
(4, 3, 3, N'Table 1C', 4, N'Bàn nhỏ trong Phòng C', 0)
INSERT [dbo].[Restaurant_Table] ([table_id], [restaurant_id], [room_id], [table_name], [max_capacity], [description], [is_booking_enabled]) VALUES 
(5, 3, 4, N'Table 1D', 10, N'Bàn gia đình trong Phòng D', 1)
INSERT [dbo].[Restaurant_Table] ([table_id], [restaurant_id], [room_id], [table_name], [max_capacity], [description], [is_booking_enabled]) VALUES 
(6, 3, NULL, N'Outdoor Table 1', 6, N'Bàn ngoài trời gần vườn', 1)
INSERT [dbo].[Restaurant_Table] ([table_id], [restaurant_id], [room_id], [table_name], [max_capacity], [description], [is_booking_enabled]) VALUES 
(7, 3, NULL, N'Outdoor Table 2', 4, N'Bàn nhỏ ngoài trời', 1)
INSERT [dbo].[Restaurant_Table] ([table_id], [restaurant_id], [room_id], [table_name], [max_capacity], [description], [is_booking_enabled]) VALUES 
(8, 3, NULL, N'Outdoor Table 3', 6, N'Bàn ngoài trời không còn sẵn sàng', 0)
SET IDENTITY_INSERT [dbo].[Restaurant_Table] OFF

GO

SET IDENTITY_INSERT [dbo].[Image] ON 

INSERT [dbo].[Image] ([image_id], [restaurant_id], [imageUrl]) VALUES (1, 3, N'https://res.cloudinary.com/do9iyczi3/image/upload/v1728022210/ijcaz7q61llt9gerwngr.jpg')
INSERT [dbo].[Image] ([image_id], [restaurant_id], [imageUrl]) VALUES (2, 3, N'https://res.cloudinary.com/do9iyczi3/image/upload/v1728022212/xy5cecupippk8j0wjcuu.jpg')

SET IDENTITY_INSERT [dbo].[Image] OFF
GO

INSERT INTO [dbo].[Category_Menu] ([restaurant_id], [category_menu_name]) 
VALUES 
(3, N'Món chính'),
(3, N'Món phụ'),
(3, N'Khai vị'),
(3, N'Tráng miệng'),
(3, N'Đồ uống');


GO

INSERT INTO [dbo].[Menu] ([restaurant_id], [category_menu_id], [dish_name], [price], [status], [image], [description]) 
VALUES
(3, 1, N'Phở bò', 50000.00, N'Active', N'https://files.elfsight.com/storage/b27fdf3d-b477-40ce-84d4-ddcade571fb4/f299acaf-76fe-4e14-aa67-fff28c5fdea0.jpeg', N'Phở bò truyền thống với nước dùng đặc biệt'),
(3, 1, N'Cơm gà', 45000.00, N'Active', N'https://files.elfsight.com/storage/b27fdf3d-b477-40ce-84d4-ddcade571fb4/422f8da7-6463-4944-a7b2-f7dfe5d5d9b8.jpeg', N'Cơm gà chiên giòn kèm salad'),
(3, 2, N'Khoai tây chiên', 30000.00, N'Active', N'https://files.elfsight.com/storage/b27fdf3d-b477-40ce-84d4-ddcade571fb4/422f8da7-6463-4944-a7b2-f7dfe5d5d9b8.jpeg', N'Khoai tây chiên giòn rụm'),
(3, 2, N'Nấm xào tỏi', 35000.00, N'Active', N'https://files.elfsight.com/storage/b27fdf3d-b477-40ce-84d4-ddcade571fb4/422f8da7-6463-4944-a7b2-f7dfe5d5d9b8.jpeg', N'Nấm xào với tỏi thơm lừng'),
(3, 3, N'Súp gà', 40000.00, N'Active', N'https://files.elfsight.com/storage/b27fdf3d-b477-40ce-84d4-ddcade571fb4/422f8da7-6463-4944-a7b2-f7dfe5d5d9b8.jpeg', N'Súp gà với ngô và rau củ'),
(3, 3, N'Salad trộn', 35000.00, N'Active', N'https://files.elfsight.com/storage/b27fdf3d-b477-40ce-84d4-ddcade571fb4/422f8da7-6463-4944-a7b2-f7dfe5d5d9b8.jpeg', N'Salad rau củ tươi ngon'),
(3, 4, N'Chè khúc bạch', 30000.00, N'Active', N'https://files.elfsight.com/storage/b27fdf3d-b477-40ce-84d4-ddcade571fb4/422f8da7-6463-4944-a7b2-f7dfe5d5d9b8.jpeg', N'Món chè khúc bạch thơm mát'),
(3, 4, N'Bánh flan', 25000.00, N'Active', N'https://files.elfsight.com/storage/b27fdf3d-b477-40ce-84d4-ddcade571fb4/422f8da7-6463-4944-a7b2-f7dfe5d5d9b8.jpeg', N'Bánh flan mềm mịn, thơm ngon'),
(3, 5, N'Trá sữa trân châu', 40000.00, N'Active', N'https://files.elfsight.com/storage/b27fdf3d-b477-40ce-84d4-ddcade571fb4/422f8da7-6463-4944-a7b2-f7dfe5d5d9b8.jpeg', N'Trá sữa với trân châu đen dai giòn'),
(3, 5, N'Cafe sữa đá', 35000.00, N'Active', N'https://files.elfsight.com/storage/b27fdf3d-b477-40ce-84d4-ddcade571fb4/422f8da7-6463-4944-a7b2-f7dfe5d5d9b8.jpeg', N'Cà phê sữa đá kiểu Việt Nam');

GO


GO
-- 1. Giảm giá theo món cho tất cả khách hàng
INSERT INTO [dbo].[Discount] 
(restaurant_id, discount_percentage, discount_name, applicable_to, apply_type, 
min_order_value, max_order_value, scope, start_date, end_date, description, is_active, quantity)
VALUES
(3, 0, N'Giảm giá theo món - Tất cả khách hàng', N'All Customers', N'All Orders', 
NULL, NULL, N'Per Service', '2024-10-16 00:00:00', '2024-12-31 23:59:59', 
N'Giảm giá riêng từng món cho tất cả khách hàng', 1, 100);

-- Thêm vào bảng Discount_Menu (discount_id = 1)
INSERT INTO [dbo].[Discount_Menu] (discount_id, menu_id, discount_menu_percentage)
VALUES 
(1, 1, 10.00), (1, 2, 10.00), (1, 3, 10.00), (1, 4, 10.00), 
(1, 5, 10.00), (1, 6, 10.00), (1, 7, 10.00), (1, 8, 10.00), 
(1, 9, 10.00), (1, 10, 10.00);

-- 2. Giảm giá cho khách hàng mới theo từng món
INSERT INTO [dbo].[Discount] 
(restaurant_id, discount_percentage, discount_name, applicable_to, apply_type, 
min_order_value, max_order_value, scope, start_date, end_date, description, is_active, quantity)
VALUES
(3, 0, N'Giảm giá món - Khách hàng mới', N'New Customer', N'All Orders', 
NULL, NULL, N'Per Service', '2024-10-16 00:00:00', '2024-11-30 23:59:59', 
N'Giảm riêng từng món cho khách hàng mới', 1, 50);

-- Thêm vào bảng Discount_Menu (discount_id = 2)
INSERT INTO [dbo].[Discount_Menu] (discount_id, menu_id, discount_menu_percentage)
VALUES 
(2, 1, 15.00), (2, 2, 15.00), (2, 3, 15.00), (2, 4, 15.00), 
(2, 5, 15.00), (2, 6, 15.00), (2, 7, 15.00), (2, 8, 15.00), 
(2, 9, 15.00), (2, 10, 15.00);

-- 3. Giảm giá theo món cho khách hàng thân thiết
INSERT INTO [dbo].[Discount] 
(restaurant_id, discount_percentage, discount_name, applicable_to, apply_type, 
min_order_value, max_order_value, scope, start_date, end_date, description, is_active, quantity)
VALUES
(3, 0, N'Giảm giá món - Khách hàng thân thiết', N'Loyal Customer', N'All Orders', 
NULL, NULL, N'Per Service', '2024-11-01 00:00:00', '2024-12-25 23:59:59', 
N'Giảm giá cho từng món với khách hàng thân thiết', 1, 30);

-- Thêm vào bảng Discount_Menu (discount_id = 3)
INSERT INTO [dbo].[Discount_Menu] (discount_id, menu_id, discount_menu_percentage)
VALUES 
(3, 1, 5.00), (3, 2, 5.00), (3, 3, 5.00), (3, 4, 5.00), 
(3, 5, 5.00), (3, 6, 5.00), (3, 7, 5.00), (3, 8, 5.00), 
(3, 9, 5.00), (3, 10, 5.00);

-- 4. Giảm giá đặc biệt cho món cho khách hàng mới
INSERT INTO [dbo].[Discount] 
(restaurant_id, discount_percentage, discount_name, applicable_to, apply_type, 
min_order_value, max_order_value, scope, start_date, end_date, description, is_active, quantity)
VALUES
(3, 0, N'Giảm giá đặc biệt theo món', N'New Customer', N'All Orders', 
NULL, NULL, N'Per Service', '2024-10-16 00:00:00', '2024-12-01 23:59:59', 
N'Áp dụng giảm giá đặc biệt từng món', 1, 20);

-- Thêm vào bảng Discount_Menu (discount_id = 4)
INSERT INTO [dbo].[Discount_Menu] (discount_id, menu_id, discount_menu_percentage)
VALUES 
(4, 1, 20.00), (4, 2, 20.00), (4, 3, 20.00), (4, 4, 20.00), 
(4, 5, 20.00), (4, 6, 20.00), (4, 7, 20.00), (4, 8, 20.00), 
(4, 9, 20.00), (4, 10, 20.00);

-- 5. Giảm giá riêng từng món cho tất cả khách hàng
INSERT INTO [dbo].[Discount] 
(restaurant_id, discount_percentage, discount_name, applicable_to, apply_type, 
min_order_value, max_order_value, scope, start_date, end_date, description, is_active, quantity)
VALUES
(3, 0, N'Tất cả khách hàng - Giảm giá từng món', N'All Customers', N'All Orders', 
NULL, NULL, N'Per Service', '2024-10-20 00:00:00', '2024-12-31 23:59:59', 
N'Giảm giá theo từng món riêng lẻ', 1, 75);

-- Thêm vào bảng Discount_Menu (discount_id = 5)
INSERT INTO [dbo].[Discount_Menu] (discount_id, menu_id, discount_menu_percentage)
VALUES 
(5, 1, 10.00), (5, 2, 10.00), (5, 3, 10.00), (5, 4, 10.00), 
(5, 5, 10.00), (5, 6, 10.00), (5, 7, 10.00), (5, 8, 10.00), 
(5, 9, 10.00), (5, 10, 10.00);


-- 1. Giảm giá cho tất cả khách hàng trên toàn bộ đơn hàng
INSERT INTO [dbo].[Discount] 
(restaurant_id, discount_percentage, discount_name, applicable_to, apply_type, 
min_order_value, max_order_value, scope, start_date, end_date, description, is_active, quantity)
VALUES
(3, 10.00, N'Tất cả khách hàng - Giảm 10%', N'All Customers', N'All Orders', 
NULL, NULL, N'Entire Order', '2024-10-16 00:00:00', '2024-12-31 23:59:59', 
N'Giảm 10% cho mọi đơn hàng', 1, 100);

-- 2. Giảm giá cho khách hàng mới trong khoảng giá trị đơn hàng
INSERT INTO [dbo].[Discount] 
(restaurant_id, discount_percentage, discount_name, applicable_to, apply_type, 
min_order_value, max_order_value, scope, start_date, end_date, description, is_active, quantity)
VALUES
(3, 20.00, N'Khách hàng mới - Giảm 20%', N'New Customer', N'Order Value Range', 
500000, 3000000, N'Entire Order', '2024-10-16 00:00:00', '2024-12-31 23:59:59', 
N'Giảm 20% cho đơn hàng từ 500,000 đến 3,000,000', 1, 50);

-- 3. Giảm giá theo món cho khách hàng thân thiết
INSERT INTO [dbo].[Discount] 
(restaurant_id, discount_percentage, discount_name, applicable_to, apply_type, 
min_order_value, max_order_value, scope, start_date, end_date, description, is_active, quantity)
VALUES
(3, NULL, N'Khách hàng thân thiết - Giảm giá theo món', N'Loyal Customer', 
N'All Orders', NULL, NULL, N'Per Service', '2024-10-16 00:00:00', '2024-11-30 23:59:59', 
N'Áp dụng giảm giá riêng cho từng món', 1, 30);

-- 4. Giảm giá cho tất cả khách hàng - toàn bộ đơn hàng
INSERT INTO [dbo].[Discount] 
(restaurant_id, discount_percentage, discount_name, applicable_to, apply_type, 
min_order_value, max_order_value, scope, start_date, end_date, description, is_active, quantity)
VALUES
(3, 5.00, N'Tất cả khách hàng - Giảm 5%', N'All Customers', N'All Orders', 
NULL, NULL, N'Entire Order', '2024-10-20 00:00:00', '2024-12-25 23:59:59', 
N'Giảm 5% cho đơn hàng', 1, 70);

-- 5. Giảm giá 15% cho khách hàng thân thiết với giá trị đơn hàng từ 1 triệu đến 5 triệu
INSERT INTO [dbo].[Discount] 
(restaurant_id, discount_percentage, discount_name, applicable_to, apply_type, 
min_order_value, max_order_value, scope, start_date, end_date, description, is_active, quantity)
VALUES
(3, 15.00, N'Khách hàng thân thiết - Giảm 15%', N'Loyal Customer', N'Order Value Range', 
1000000, 5000000, N'Entire Order', '2024-10-18 00:00:00', '2024-11-20 23:59:59', 
N'Giảm 15% cho đơn hàng lớn', 1, 40);

-- 6. Khuyến mãi món riêng cho khách hàng mới
INSERT INTO [dbo].[Discount] 
(restaurant_id, discount_percentage, discount_name, applicable_to, apply_type, 
min_order_value, max_order_value, scope, start_date, end_date, description, is_active, quantity)
VALUES
(3, NULL, N'Món riêng - Khuyến mãi khách hàng mới', N'New Customer', N'All Orders', 
NULL, NULL, N'Per Service', '2024-10-16 00:00:00', '2024-12-01 23:59:59', 
N'Giảm giá cho từng món riêng cho khách hàng mới', 1, 20);

-- 7. Giảm giá 25% cho khách hàng mới với đơn hàng từ 1 triệu đến 2 triệu
INSERT INTO [dbo].[Discount] 
(restaurant_id, discount_percentage, discount_name, applicable_to, apply_type, 
min_order_value, max_order_value, scope, start_date, end_date, description, is_active, quantity)
VALUES
(3, 25.00, N'Khách hàng mới - Giảm 25%', N'New Customer', N'Order Value Range', 
1000000, 2000000, N'Entire Order', '2024-10-20 00:00:00', '2024-11-15 23:59:59', 
N'Giảm 25% cho đơn hàng lớn', 1, 35);

-- 8. Giảm giá món theo dịch vụ cho khách hàng thân thiết
INSERT INTO [dbo].[Discount] 
(restaurant_id, discount_percentage, discount_name, applicable_to, apply_type, 
min_order_value, max_order_value, scope, start_date, end_date, description, is_active, quantity)
VALUES
(3, NULL, N'Giảm món cho khách hàng thân thiết', N'Loyal Customer', N'All Orders', 
NULL, NULL, N'Per Service', '2024-11-01 00:00:00', '2024-12-31 23:59:59', 
N'Khuyến mãi theo món cho khách hàng thân thiết', 1, 10);

-- 9. Giảm 30% cho tất cả khách hàng trên toàn bộ đơn hàng
INSERT INTO [dbo].[Discount] 
(restaurant_id, discount_percentage, discount_name, applicable_to, apply_type, 
min_order_value, max_order_value, scope, start_date, end_date, description, is_active, quantity)
VALUES
(3, 30.00, N'Tất cả khách hàng - Giảm 30%', N'All Customers', N'All Orders', 
NULL, NULL, N'Entire Order', '2024-11-10 00:00:00', '2024-12-25 23:59:59', 
N'Giảm 30% cho mọi đơn hàng', 1, 90);

-- 10. Giảm giá món đặc biệt theo dịch vụ cho khách hàng mới
INSERT INTO [dbo].[Discount] 
(restaurant_id, discount_percentage, discount_name, applicable_to, apply_type, 
min_order_value, max_order_value, scope, start_date, end_date, description, is_active, quantity)
VALUES
(3, NULL, N'Giảm món đặc biệt', N'New Customer', N'All Orders', 
NULL, NULL, N'Per Service', '2024-10-16 00:00:00', '2024-11-30 23:59:59', 
N'Khuyến mãi đặc biệt theo từng món', 1, 15);

GO 

INSERT INTO [dbo].[Notification] (uid, content, type, is_read, created_at) 
VALUES 
(2, N'Đơn hàng của bạn đã được xác nhận.', 'Order', 0, GETDATE()),
(2, N'Nhà hàng đã chuẩn bị xong đơn hàng của bạn.', 'Order', 0, GETDATE()),
(2, N'Thông báo khuyến mãi 20% cho tất cả các đơn hàng.', 'Promotion', 0, GETDATE()),
(2, N'Tài khoản của bạn đã được cập nhật thông tin thành công.', 'Account', 1, GETDATE()),
(2, N'Mời bạn đánh giá dịch vụ của chúng tôi.', 'Feedback', 0, GETDATE()),

(3, N'Đơn hàng của bạn đã được giao thành công.', 'Order', 1, GETDATE()),
(3, N'Thông báo khuyến mãi 10% cho khách hàng thân thiết.', 'Promotion', 0, GETDATE()),
(3, N'Tài khoản của bạn vừa được đăng nhập từ thiết bị lạ.', 'Security', 0, GETDATE()),
(3, N'Nhà hàng đối tác mới đã được thêm vào danh sách.', 'Info', 0, GETDATE()),
(3, N'Hóa đơn thanh toán của bạn đã được xuất.', 'Invoice', 1, GETDATE());
GO

-- Giả sử restaurant có uid = 3 và customer có uid = 2
INSERT INTO [dbo].[ChatBox] (customer_id, restaurant_id) 
VALUES (2, 3);
GO

-- Thêm các tin nhắn vào chat box với id = 1
INSERT INTO [dbo].[Chat] (chat_box_id, content, chat_by) 
VALUES 
(1, N'Xin chào nhà hàng! Nhà hàng có mở cửa không?', 2),  -- Tin nhắn từ customer (uid = 2)
(1, N'Chào bạn! Nhà hàng chúng tôi đang mở. Bạn cần hỗ trợ gì?', 3), -- Tin nhắn từ restaurant (uid = 3)
(1, N'Mình có thể đặt bàn cho 2 người lúc 7 giờ tối không?', 2),
(1, N'Dạ được, nhà hàng sẽ chuẩn bị bàn cho bạn. Cảm ơn đã liên hệ!', 3),
(1, N'Cảm ơn nhà hàng nhé!', 2),
(1, N'Không có gì, hẹn gặp lại bạn!', 3);
GO

-- Giả sử restaurant có uid = 3 và customer có uid = 2
INSERT INTO [dbo].[Wishlist] (customer_id, restaurant_id) 
VALUES (2, 3);
GO

-- Thêm 10 feedback từ customer_id = 2 cho restaurant_id = 3
INSERT INTO [dbo].[Feedback] (customer_id, restaurant_id, star, content, status)
VALUES 
(2, 3, 5, N'Thức ăn ngon, phục vụ chu đáo.', N'Approved'),
(2, 3, 4, N'Không gian đẹp nhưng hơi ồn.', N'Approved'),
(2, 3, 5, N'Rất hài lòng với dịch vụ.', N'Approved'),
(2, 3, 3, N'Món ăn bình thường, chưa đặc sắc.', N'Pending'),
(2, 3, 2, N'Nhân viên phục vụ không nhiệt tình.', N'Rejected'),
(2, 3, 4, N'Giá cả hợp lý, sẽ quay lại.', N'Approved'),
(2, 3, 1, N'Rất thất vọng về chất lượng.', N'Rejected'),
(2, 3, 3, N'Cần cải thiện thời gian chuẩn bị món.', N'Pending'),
(2, 3, 5, N'Món tráng miệng tuyệt vời.', N'Approved'),
(2, 3, 4, N'Nhà hàng có nhiều món ngon.', N'Approved');
GO
