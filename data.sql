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
INSERT [dbo].[Restaurant] ([uid], [category_restaurant_id], [name_owner], [name_res], [logo], [open_time], [close_time], [address], [phone], [description], [subdescription], [location], [discount], [max_capacity], [price], [is_booking_enabled], [first_fee_percent], [returning_fee_percent], [cancellation_fee_percent]) VALUES (3, 7, N'Đỗ Văn Đạt', N'Mer. coffee & tea ', N'https://res.cloudinary.com/do9iyczi3/image/upload/v1728975519/hesdfxvomskasmtj0zea.jpg', CAST(N'08:00:00' AS Time), CAST(N'23:00:00' AS Time), N'A3-15 KDG Cổng Chung, Tân Hội, Đan Phượng, Hà Nội', N'0968519615', NULL, NULL, N'Hà Nội', CAST(0.00 AS Decimal(5, 2)), 30, CAST(20000.00 AS Decimal(18, 2)), 1, CAST(100.00 AS Decimal(5, 2)), CAST(100.00 AS Decimal(5, 2)), CAST(100.00 AS Decimal(5, 2)))
GO
INSERT [dbo].[Admin] ([uid], [name], [phone], [dob], [image]) VALUES (1, N'TOPDER', N'0968519615', CAST(N'2024-10-15' AS Date), N'https://res.cloudinary.com/do9iyczi3/image/upload/v1726643328/LOGO-TOPDER_qonl9l.png')
GO
SET IDENTITY_INSERT [dbo].[Wallet] ON 

INSERT [dbo].[Wallet] ([wallet_id], [uid], [wallet_balance], [bank_code], [account_no], [account_name], [otp_code]) VALUES (1, 1, CAST(0.00 AS Decimal(18, 2)), NULL, NULL, NULL, NULL)
INSERT [dbo].[Wallet] ([wallet_id], [uid], [wallet_balance], [bank_code], [account_no], [account_name], [otp_code]) VALUES (2, 2, CAST(0.00 AS Decimal(18, 2)), NULL, NULL, NULL, NULL)
INSERT [dbo].[Wallet] ([wallet_id], [uid], [wallet_balance], [bank_code], [account_no], [account_name], [otp_code]) VALUES (3, 3, CAST(0.00 AS Decimal(18, 2)), NULL, NULL, NULL, NULL)
SET IDENTITY_INSERT [dbo].[Wallet] OFF
GO
