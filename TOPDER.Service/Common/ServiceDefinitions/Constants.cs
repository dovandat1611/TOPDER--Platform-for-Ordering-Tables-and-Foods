using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Common.ServiceDefinitions
{
    public class Constants
    {   

        public class Common_Status
        {
            public static string ACTIVE = "Active";
            public static string INACTIVE = "In-Active";
        }

        public class Booking_Status
        {
            public static string ACTIVE = "Active";
            public static string INACTIVE = "In-Active";
            public static string CANCELLED = "Cancelled";
        }

        public class Email_Subject
        {
            public static string REGISTER = "Chào Mừng Bạn Đến Với TOPDER";
            public static string VERIFY = "Xác Thực Email - TOPDER";
            public static string OTP = "Mã Xác Nhận OTP - TOPDER";
            public static string CONTACT = "Liên Hệ Với TOPDER";
            public static string CONTACT_REGISTER = "Đăng Ký Nhà Hàng Với TOPDER";
            public static string ORDERCONFIRM = "Xác Nhận Đặt Hàng - TOPDER";
            public static string UPDATESTATUS = "Cập Nhật Trạng Thái Đơn Hàng Của Bạn - TOPDER";
            public static string NEWORDER = "Cửa Hàng Của Bạn Có Đơn Hàng Mới - TOPDER";
        }

        public class Is_Null
        {
            public static string ISNULL = "N/A";
        }

        public class Transaction_Type
        {
            public static string RECHARGE = "Recharge";
            public static string WITHDRAW = "Withdraw";
            public static string SYSTEMADD = "SystemAdd";
            public static string SYSTEMSUBTRACT = "SystemSubtract";
        }

        public class ExternalProvider
        {
            public static string GOOGLE = "Google";
        }

        public class PaymentGateway
        {
            public static string VNPAY = "VNPAY";
            public static string VIETQR = "VIETQR";
            public static string ISBALANCE = "ISBALANCE";
        }

        public class Contact_Topic
        {
            public static string RESTAURANT_REGISTER = "RestaurantRegister";
            public static string OTHER = "Other";
        }

        public class Payment_Status
        {
            public static string PENDING = "Pending";      
            public static string CANCELLED = "Cancelled";    
            public static string SUCCESSFUL = "Successful";   
        }

        public class Payment_Descriptions
        {
            public static string WithdrawalDescription(decimal price)
            {
                return $"Tài khoản rút đăng ký rút số tiền {price}.";
            }

            public static string RechargeDescription(decimal price)
            {
                return $"Tài khoản nạp số tiền {price}.";
            }

            public static string RechargeVIETQRDescription()
            {
                return $"Tai khoan nap tien";
            }

            public static string SystemSubtractDescription(decimal price)
            {
                return $"Hệ thống trừ {price} vào ví.";
            }
            public static string SystemAddtractDescription(decimal price)
            {
                return $"Hệ thống cộng {price} vào ví.";
            }
        }

        public class Notification_Type
        {
            public static string SYSTEM_ADD = "Hệ Thống Cộng Tiền Vào Ví";
            public static string SYSTEM_SUB = "Hệ Thống Trừ Tiền Từ Ví";
        }

        public class Notification_Content
        {
            public static string SYSTEM_ADD(decimal price)
            {
                return $"Hệ thống cộng số tiền {price} vào ví của bạn.";
            }
            public static string SYSTEM_SUB(decimal price)
            {
                return $"Hệ thống hỗ trợ rút số tiền {price} từ ví của bạn thành công.";
            }
        }

        public class Log_Type
        {
            public static string FEE_PERCENT = "Fee-Percent";
        }

        public class Default_Avatar
        {
            public static string ADMIN = "https://res.cloudinary.com/do9iyczi3/image/upload/v1726643328/LOGO-TOPDER_qonl9l.png";
            public static string CUSTOMER = "https://res.cloudinary.com/do9iyczi3/image/upload/v1728023034/default-avatar-profile-icon_ecq8w3.jpg";
        }

        public class User_Role
        {
            public static string ADMIN = "Admin";
            public static string CUSTOMER = "Customer";
            public static string RESTAURANT = "Restaurant";
        }

        public class Order_Type
        {
            public static string RESERVATION = "Reservation";
            public static string DELIVERY = "Delivery ";
        }

        public class Order_Status
        {
            public static string PENDING = "Pending";
            public static string CONFIRM = "Confirm";
            public static string PAID = "Paid";
            public static string COMPLETE = "Complete"; 
            public static string CANCEL = "Cancel"; 
        }

        public class DiscountApplicableTo
        {
            public static string ALL_CUSTOMERS = "All Customers";
            public static string NEW_CUSTOMER = "New Customer";
            public static string LOYAL_CUSTOMER = "Loyal Customer";
        }

        public class DiscountApplyType
        {
            public static string ALL_ORDERS = "All Orders";
            public static string ORDER_VALUE_RANGE = "Order Value Range";
        }

        public class DiscountScope
        {
            public static string ENTIRE_ORDER = "Entire Order";
            public static string PER_SERVICE = "Per Service";
        }


        public class Order_PaymentContent
        {
            public static string PaymentContent (int id, int restaurantID)
            {
                return $"Tài khoản mang ID {id} đặt bàn ở nhà hàng có ID {restaurantID}.";
            }
            public static string PaymentContentVIETQR()
            {
                return $"Thanh toan dat ban TOPDER";
            }
        }
        public class Booking_PaymentContent
        {
            public static string PaymentContentVIETQR()
            {
                return $"quang cao TOPDER";
            }
        }


    }
}
