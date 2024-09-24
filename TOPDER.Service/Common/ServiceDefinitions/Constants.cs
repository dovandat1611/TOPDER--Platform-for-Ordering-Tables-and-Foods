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

        public class Is_Null
        {
            public static string ISNULL = "N/A";
        }

        public class Transaction_Type
        {
            public static string DEPOSIT = "Deposit";
            public static string WITHDRAW = "Withdraw";
            public static string SYSTEMADD = "SystemAdd";
            public static string SYSTEMSUBTRACT = "SystemSubtract";
        }

        public class Transaction_Status
        {
            public static string PENDING = "Pending";
            public static string COMPLETE = "Complete";
        }

        public class Log_Type
        {
            public static string FEE_PERCENT = "Fee-Percent";
        }

        public class User_Role
        {
            public static string ADMIN = "Admin";
            public static string CUSTOMER = "Customer";
            public static string RESTAURANT = "Restaurant";
        }

    }
}
