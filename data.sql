USE [TopderDB]
GO
SET IDENTITY_INSERT [dbo].[Role] ON 

INSERT [dbo].[Role] ([role_id], [name]) VALUES (1, N'Admin')
INSERT [dbo].[Role] ([role_id], [name]) VALUES (2, N'Restaurant')
INSERT [dbo].[Role] ([role_id], [name]) VALUES (3, N'Customer')
SET IDENTITY_INSERT [dbo].[Role] OFF
GO
SET IDENTITY_INSERT [dbo].[User] ON 

INSERT [dbo].[User] ([uid], [role_id], [email], [password], [otp_code], [is_verify], [status], [created_at], [is_external_login]) VALUES (1, 1, N'topder.vn@gmail.com', N'$2a$11$bCy3gf5Qca1awsszGPYdke3I9bIvNCNWVtwRxHfcclrOluakbsKNK', N'', 1, N'Active', CAST(N'2024-10-15T13:54:18.213' AS DateTime), 0)
INSERT [dbo].[User] ([uid], [role_id], [email], [password], [otp_code], [is_verify], [status], [created_at], [is_external_login]) VALUES (2, 3, N'datdvhe161664@fpt.edu.vn', N'$2a$11$SSAKsdpn7NRGc8cZCjcM1ugRG1CmiEk4ZMvNbgzmS6OVu5C5LPSwC', N'', 1, N'Active', CAST(N'2024-10-15T13:56:01.427' AS DateTime), 0)
INSERT [dbo].[User] ([uid], [role_id], [email], [password], [otp_code], [is_verify], [status], [created_at], [is_external_login]) VALUES (3, 2, N'dovandat1611@gmail.com', N'$2a$11$yK3dvHfZQjMST23LdKyDjeGdSwaT8qmj3YcrwxzKMCw0HUFIl9xNS', N'', 1, N'In-Active', CAST(N'2024-10-15T13:58:38.483' AS DateTime), 0)
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
INSERT [dbo].[Restaurant] ([uid], [category_restaurant_id], [name_owner], [name_res], [logo], [open_time], [close_time], [address], [phone], [description], [subdescription], [province_city],[district],[commune], [discount], [max_capacity], [price], [is_booking_enabled], [first_fee_percent], [returning_fee_percent], [cancellation_fee_percent]) VALUES (3, 7, N'Đỗ Văn Đạt', N'Mer. coffee & tea ', N'https://res.cloudinary.com/do9iyczi3/image/upload/v1728975519/hesdfxvomskasmtj0zea.jpg', CAST(N'08:00:00' AS Time), CAST(N'23:00:00' AS Time), N'A3-15 KDG Cổng Chung, Tân Hội, Đan Phượng, Hà Nội', N'0968519615', NULL, NULL, 01, 276, 10000, CAST(0.00 AS Decimal(5, 2)), 30, CAST(20000.00 AS Decimal(18, 2)), 1, CAST(100.00 AS Decimal(5, 2)), CAST(100.00 AS Decimal(5, 2)), CAST(100.00 AS Decimal(5, 2)))
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