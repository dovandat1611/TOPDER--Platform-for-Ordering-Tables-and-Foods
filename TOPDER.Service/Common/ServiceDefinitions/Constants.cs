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

        public class Email_Subject
        {
            public static string REGISTER = "Chào Mừng Bạn Đến Với TOPDER";
            public static string VERIFY = "Xác Thực Email - TOPDER";
            public static string OTP = "Mã Xác Nhận OTP - TOPDER";
            public static string CONTACT = "Liên Hệ Với TOPDER";
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

        public class PaymentGateway
        {
            public static string VNPAY = "VNPAY";
            public static string VIETQR = "VIETQR";
            public static string ISBALANCE = "ISBALANCE";
        }

        public class Payment_Status
        {
            public static string PENDING = "Pending";      
            public static string CANCELLED = "Cancelled";    
            public static string SUCCESSFUL = "Successful";   
        }

        public class Payment_Descriptions
        {
            public static string WithdrawalDescription(string name, int id)
            {
                return $"Tài khoản có tên {name} mang ID {id} rút tiền.";
            }

            public static string RechargeDescription(string name, int id)
            {
                return $"Tài khoản có tên {name} mang ID {id} nạp tiền.";
            }
            public static string SystemSubtractDescription(string name, int id)
            {
                return $"Hệ thống trừ tiền tài khoản có tên {name} mang ID {id}.";
            }
            public static string SystemAddtractDescription(string name, int id)
            {
                return $"Hệ thống cộng tiền tài khoản có tên {name} mang ID {id}.";
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

        public class Order_PaymentContent
        {
            public static string PaymentContent (int id, int restaurantID)
            {
                return $"Tài khoản mang ID {id} đặt bàn ở nhà hàng có ID {restaurantID}.";
            }
        }


    }
}
