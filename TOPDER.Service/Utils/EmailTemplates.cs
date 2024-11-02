using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Service.Dtos.Email;

namespace TOPDER.Service.Utils
{
    public class EmailTemplates
    {

        public static string Verify(string name, int uid)
        {
            return $@"
            <!DOCTYPE html>
            <html>
            <head>
                <meta name='viewport' content='width=device-width, initial-scale=1'>
            </head>
            <body style='font-family: Arial, sans-serif; background-color: #f4f4f4; margin: 0; padding: 0;'>
                <div style='margin: 20px auto; max-width: 600px; width: 100%; background-color: #ffffff; border-radius: 8px; box-shadow: 0 4px 20px rgba(0, 0, 0, 0.1); overflow: hidden;'>
                    <div style='background-color: #f29034; padding: 20px; text-align: center; color: #ffffff; font-size: 1.5rem; font-weight: bold;'>
                        Xác Thực Email - <span style='color: #ffffff;'>TOPDER!</span>
                    </div>
                    <div style='padding: 20px; text-align: center; color: #272241;'>
                        <img src='https://res.cloudinary.com/do9iyczi3/image/upload/v1726643328/LOGO-TOPDER_qonl9l.png' alt='Logo TOPDER' width='150' style='display: block; margin: 0 auto;' />
                        <div style='background-color: #f9f9f9; padding: 20px; border-radius: 5px; box-shadow: 0 2px 8px rgba(8, 120, 211, 0.1); text-align: left;'>
                            <p>Xin chào, <span style='font-weight: bold; color: #272241;'>{name}</span></p>
                            <p>Chúng tôi thấy bạn đang gửi yêu cầu xác thực địa chỉ email này để tạo tài khoản trên <span style='font-weight: bold; color: #272241;'>TOPDER</span>.</p>
                            <p>Để xác nhận email đăng ký, bạn vui lòng click vào nút &quot;Xác thực&quot; bên dưới.</p>
                            <br><br>
                            <div style='text-align: center;'>
                                <a href='https://localhost:7134/api/User/VerifyAccount/{uid}' style='display: inline-block; background-color: #f29034; color: #ffffff; padding: 10px 20px; border-radius: 5px; text-decoration: none; font-weight: bold;'>
                                  Xác Thực
                                </a>
                            </div>
                            <br><br>
                            <p>Để biết thêm thông tin, vui lòng truy cập <a href='https://www.topder.vn' style='color: #f29034; text-decoration: none;'>website</a> của chúng tôi.</p>
                            <p>Xin chân thành cảm ơn!</p>
                            <p>Hân hạnh,</p>
                            <p style='font-weight: 700; color: #272241;'>Đội ngũ TOPDER</p>
                        </div>
                    </div>
                    <div style='background-color: #f29034; padding: 10px; text-align: center; color: #ffffff; font-size: 0.875rem; font-weight: bold;'>
                        © 2024 | Bản quyền thuộc về TOPDER.
                    </div>
                </div>
            </body>
            </html>";
        }

        public static string OTP(string name, string otp)
        {
            return $@"
            <!DOCTYPE html>
            <html>
            <head>
                <meta name='viewport' content='width=device-width, initial-scale=1'>
            </head>
            <body style='font-family: Arial, sans-serif; background-color: #f4f4f4; margin: 0; padding: 0;'>
                <div style='margin: 20px auto; max-width: 600px; width: 100%; background-color: #ffffff; border-radius: 8px; box-shadow: 0 4px 20px rgba(0, 0, 0, 0.1); overflow: hidden;'>
                    <div style='background-color: #f29034; padding: 20px; text-align: center; color: #ffffff; font-size: 1.5rem; font-weight: bold;'>
                        OTP <span style='color: #ffffff;'>TOPDER!</span>
                    </div>
                    <div style='padding: 20px; text-align: center; color: #272241;'>
                        <img src='https://res.cloudinary.com/do9iyczi3/image/upload/v1726643328/LOGO-TOPDER_qonl9l.png' alt='Logo TOPDER' width='150' style='display: block; margin: 0 auto;' />
                        <div style='background-color: #f9f9f9; padding: 20px; border-radius: 5px; box-shadow: 0 2px 8px rgba(8, 120, 211, 0.1); text-align: left;'>
                            <p>Xin chào, <span style='font-weight: bold; color: #272241;'>{name}</span></p>
                            <p>Cảm ơn bạn đã đăng ký tài khoản với <span style='font-weight: bold; color: #272241;'>TOPDER</span>.</p>
                            <p>Mã xác thực của bạn là: <span style='color: #272241; font-weight: bold;'>{otp}</span></p>
                            <p>Vui lòng nhập mã này để xác minh tài khoản của bạn.</p>
                            <p>Để biết thêm thông tin, vui lòng truy cập <a href='https://www.topder.vn' style='color: #f29034; text-decoration: none;'>website</a> của chúng tôi.</p>
                            <p>Xin chân thành cảm ơn!</p>
                            <p>Hân hạnh,</p>
                            <p style='font-weight: 700; color: #272241;'>Đội ngũ TOPDER</p>
                        </div>
                    </div>
                    <div style='background-color: #f29034; padding: 10px; text-align: center; color: #ffffff; font-size: 0.875rem; font-weight: bold;'>
                        © 2024 | Bản quyền thuộc về TOPDER.
                    </div>
                </div>
            </body>
            </html>";
        }

        public static string Contact(string name)
        {
            return $@"
            <!DOCTYPE html>
            <html>
            <head>
                <meta name='viewport' content='width=device-width, initial-scale=1'>
            </head>
            <body style='font-family: Arial, sans-serif; background-color: #f4f4f4; margin: 0; padding: 0;'>
                <div style='margin: 20px auto; max-width: 600px; width: 100%; background-color: #ffffff; border-radius: 8px; box-shadow: 0 4px 20px rgba(0, 0, 0, 0.1); overflow: hidden;'>
                    <div style='background-color: #f29034; padding: 20px; text-align: center; color: #ffffff; font-size: 1.5rem; font-weight: bold;'>
                        Liên hệ với <span style='color: #ffffff;'>TOPDER</span>
                    </div>
                    <div style='padding: 20px; text-align: center; color: #272241;'>
                        <img src='https://res.cloudinary.com/do9iyczi3/image/upload/v1726643328/LOGO-TOPDER_qonl9l.png' alt='Logo TOPDER' width='150' style='display: block; margin: 0 auto;' />
                        <div style='background-color: #f9f9f9; padding: 20px; border-radius: 5px; box-shadow: 0 2px 8px rgba(8, 120, 211, 0.1); text-align: left;'>
                            <p>Chào <span style='font-weight: bold; color: #272241;'>{name}</span>,</p>
                            <p>Cảm ơn bạn đã liên hệ với chúng tôi.</p>
                            <p>Chúng tôi đã nhận được yêu cầu của bạn và sẽ phản hồi sớm nhất có thể. Trong thời gian chờ đợi, bạn có thể truy cập <a href='https://www.topder.vn' style='color: #f29034; text-decoration: none;'>website</a> của chúng tôi để tìm hiểu thêm thông tin.</p>
                            <p>Xin chân thành cảm ơn!</p>
                            <p>Trân trọng,</p>
                            <p style='font-weight: 700; color: #272241;'>Đội ngũ TOPDER</p>
                        </div>
                    </div>
                    <div style='background-color: #f29034; padding: 10px; text-align: center; color: #ffffff; font-size: 0.875rem; font-weight: bold;'>
                        © 2024 | Bản quyền thuộc về TOPDER.
                    </div>
                </div>
            </body>
            </html>";
        }

        public static string RegisterRestaurant(string name)
        {
            return $@"
            <!DOCTYPE html>
            <html>
            <head>
                <meta name='viewport' content='width=device-width, initial-scale=1'>
            </head>
            <body style='font-family: Arial, sans-serif; background-color: #f4f4f4; margin: 0; padding: 0;'>
                <div style='margin: 20px auto; max-width: 600px; width: 100%; background-color: #ffffff; border-radius: 8px; box-shadow: 0 4px 20px rgba(0, 0, 0, 0.1); overflow: hidden;'>
                    <div style='background-color: #f29034; padding: 20px; text-align: center; color: #ffffff; font-size: 1.5rem; font-weight: bold;'>
                        Đăng ký nhà hàng với <span style='color: #ffffff;'>TOPDER</span>
                    </div>
                    <div style='padding: 20px; text-align: center; color: #272241;'>
                        <img src='https://res.cloudinary.com/do9iyczi3/image/upload/v1726643328/LOGO-TOPDER_qonl9l.png' alt='Logo TOPDER' width='150' style='display: block; margin: 0 auto;' />
                        <div style='background-color: #f9f9f9; padding: 20px; border-radius: 5px; box-shadow: 0 2px 8px rgba(8, 120, 211, 0.1); text-align: left;'>
                            <p>Chào <span style='font-weight: bold; color: #272241;'>{name}</span>,</p>
                            <p>Cảm ơn bạn đã liên hệ với chúng tôi.</p>
                            <p>Chúng tôi đã nhận được yêu cầu đăng ký nhà hàng của bạn, bạn có thể truy cập <a href='https://www.topder.vn' style='color: #f29034; text-decoration: none;'>Đăng Ký Nhà Hàng</a> của chúng tôi để tìm đăng ký nhà hàng ngay bây giờ!.</p>
                            <p>Xin chân thành cảm ơn!</p>
                            <p>Trân trọng,</p>
                            <p style='font-weight: 700; color: #272241;'>Đội ngũ TOPDER</p>
                        </div>
                    </div>
                    <div style='background-color: #f29034; padding: 10px; text-align: center; color: #ffffff; font-size: 0.875rem; font-weight: bold;'>
                        © 2024 | Bản quyền thuộc về TOPDER.
                    </div>
                </div>
            </body>
            </html>";
        }

        public static string OrderStatusUpdate(string name, string orderId, string status)
        {
            return $@"
            <!DOCTYPE html>
            <html>
            <head>
                <meta name='viewport' content='width=device-width, initial-scale=1'>
            </head>
            <body style='font-family: Arial, sans-serif; background-color: #f4f4f4; margin: 0; padding: 0;'>
                <div style='margin: 20px auto; max-width: 600px; width: 100%; background-color: #ffffff; border-radius: 8px; box-shadow: 0 4px 20px rgba(0, 0, 0, 0.1); overflow: hidden;'>
                    <div style='background-color: #f29034; padding: 20px; text-align: center; color: #ffffff; font-size: 1.5rem; font-weight: bold;'>
                        Cập Nhật Trạng Thái Đơn Hàng
                    </div>
                    <div style='padding: 20px; text-align: center; color: #272241;'>
                        <img src='https://res.cloudinary.com/do9iyczi3/image/upload/v1726643328/LOGO-TOPDER_qonl9l.png' alt='Logo TOPDER' width='150' style='display: block; margin: 0 auto;' />
                        <div style='background-color: #f9f9f9; padding: 20px; border-radius: 5px; box-shadow: 0 2px 8px rgba(8, 120, 211, 0.1); text-align: left;'>
                            <p>Kính gửi <span style='font-weight: bold; color: #272241;'>{name}</span>,</p>
                            <p>Chúng tôi muốn thông báo cho bạn về việc cập nhật trạng thái đơn hàng của bạn.</p>
                            <p>Mã Đơn Hàng: <span style='font-weight: bold; color: #272241;'>#{orderId}</span></p>
                            <p>Trạng Thái Hiện Tại: <span style='font-weight: bold; color: #272241;'>{status}</span></p>
                            <p>Để biết thêm thông tin, xin vui lòng truy cập <a href='https://www.topder.vn' style='color: #f29034; text-decoration: none;'>website</a> của chúng tôi để tìm hiểu thêm thông tin.</p>
                            <p>Trân trọng,</p>
                            <p style='font-weight: 700; color: #272241;'>Topder</p>
                        </div>
                    </div>
                    <div style='background-color: #f29034; padding: 10px; text-align: center; color: #ffffff; font-size: 0.875rem; font-weight: bold;'>
                        © 2024 | Bản quyền thuộc về TOPDER.
                    </div>
                </div>
            </body>
            </html>";
        }

        public static string NewOrder(string nameRestaurant, string orderId)
        {
            return $@"
            <!DOCTYPE html>
            <html>
            <head>
                <meta name='viewport' content='width=device-width, initial-scale=1'>
            </head>
            <body style='font-family: Arial, sans-serif; background-color: #f4f4f4; margin: 0; padding: 0;'>
                <div style='margin: 20px auto; max-width: 600px; width: 100%; background-color: #ffffff; border-radius: 8px; box-shadow: 0 4px 20px rgba(0, 0, 0, 0.1); overflow: hidden;'>
                    <div style='background-color: #f29034; padding: 20px; text-align: center; color: #ffffff; font-size: 1.5rem; font-weight: bold;'>
                        Đơn Hàng Mới!
                    </div>
                    <div style='padding: 20px; text-align: center; color: #272241;'>
                        <img src='https://res.cloudinary.com/do9iyczi3/image/upload/v1726643328/LOGO-TOPDER_qonl9l.png' alt='Logo TOPDER' width='150' style='display: block; margin: 0 auto;' />
                        <div style='background-color: #f9f9f9; padding: 20px; border-radius: 5px; box-shadow: 0 2px 8px rgba(8, 120, 211, 0.1); text-align: left;'>
                            <p>Kính gửi nhà hàng <span style='font-weight: bold; color: #272241;'>{nameRestaurant}</span>,</p>
                            <p>Chúng tôi muốn thông báo cho bạn rằng đã có đơn hàng mới.</p>
                            <p>Mã Đơn Hàng: <span style='font-weight: bold; color: #272241;'>#{orderId}</span></p>
                            <p>Để biết thêm thông tin, xin vui lòng truy cập <a href='https://www.topder.vn' style='color: #f29034; text-decoration: none;'>website</a> của chúng tôi để tìm hiểu thêm thông tin.</p>
                            <p>Trân trọng,</p>
                            <p style='font-weight: 700; color: #272241;'>Topder</p>
                        </div>
                    </div>
                    <div style='background-color: #f29034; padding: 10px; text-align: center; color: #ffffff; font-size: 0.875rem; font-weight: bold;'>
                        © 2024 | Bản quyền thuộc về TOPDER.
                    </div>
                </div>
            </body>
            </html>";
        }


        public static string Order(OrderPaidEmail orderConfirmationEmail)
        {
            string formattedDate = orderConfirmationEmail.ReservationDate.ToString("dd/MM/yyyy");
            string formattedTime = orderConfirmationEmail.ReservationTime.ToString(@"hh\:mm");

            var tablesHtml = "<ul>";

            if (orderConfirmationEmail.Rooms != null && orderConfirmationEmail.Rooms.Any())
            {
                foreach (var room in orderConfirmationEmail.Rooms)
                {
                    tablesHtml += $"<li>Phòng: {room.RoomName}</li>";
                    if (room.Tables != null && room.Tables.Any())
                    {
                        tablesHtml += "<ul>";
                        foreach (var table in room.Tables)
                        {
                            tablesHtml += $"<li>{table}</li>";
                        }
                        tablesHtml += "</ul>";
                    }
                }
            }

            if (orderConfirmationEmail.TableName != null && orderConfirmationEmail.TableName.Any())
            {
                foreach (var table in orderConfirmationEmail.TableName)
                {
                    tablesHtml += $"<li>{table}</li>";
                }
            }

            tablesHtml += "</ul>";

            return $@"
                    <!DOCTYPE html>
                    <html lang='vi'>
                    <head>
                      <meta charset='UTF-8'>
                      <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    </head>
                    <body style='font-family: Arial, sans-serif; background-color: #f4f4f4; margin: 0; padding: 0;'>
                      <div style='width: 100%; max-width: 600px; margin: 20px auto; background-color: #ffffff; border-radius: 10px; box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1); overflow: hidden;'>
                        <div style='background-color: #f29034; color: #ffffff; text-align: center; padding: 20px; font-size: 1.5rem; font-weight: bold;'>
                          Xác nhận đặt bàn thành công
                        </div>

                        <div style='padding: 20px;'>
                          <img src='https://res.cloudinary.com/do9iyczi3/image/upload/v1726643328/LOGO-TOPDER_qonl9l.png' alt='Logo TOPDER' style='display: block; margin: 20px auto; width: 120px;' />
                          <p style='color: #f29034; font-weight: bold; font-size: 1.1rem; margin: 15px 0;'>Cảm ơn bạn đã đặt bàn tại TOPDER!</p>
                          <p style='font-size: 1.2rem; font-weight: bold; margin-bottom: 10px; color: #333333;'>Thông tin đặt bàn của bạn như sau:</p>
                          <p style='margin: 8px 0; font-size: 1rem; line-height: 1.5; color: #555555; padding: 10px; background-color: #f9f9f9; border-radius: 5px;'>Mã đơn hàng: <span style='font-weight: bold; color: #333333;'>#{orderConfirmationEmail.OrderId}</span></p>
                          <p style='margin: 8px 0; font-size: 1rem; line-height: 1.5; color: #555555; padding: 10px; background-color: #f9f9f9; border-radius: 5px;'>Tên khách hàng: <span style='font-weight: bold; color: #333333;'>{orderConfirmationEmail.Name}</span></p>
                          <p style='margin: 8px 0; font-size: 1rem; line-height: 1.5; color: #555555; padding: 10px; background-color: #f9f9f9; border-radius: 5px;'>Nhà hàng: <span style='font-weight: bold; color: #333333;'>{orderConfirmationEmail.RestaurantName}</span></p>
                          <p style='margin: 8px 0; font-size: 1rem; line-height: 1.5; color: #555555; padding: 10px; background-color: #f9f9f9; border-radius: 5px;'>Số lượng khách: <span style='font-weight: bold; color: #333333;'>{orderConfirmationEmail.NumberOfGuests} người</span></p>
                          <p style='margin: 8px 0; font-size: 1rem; line-height: 1.5; color: #555555; padding: 10px; background-color: #f9f9f9; border-radius: 5px;'>Ngày đặt bàn: <span style='font-weight: bold; color: #333333;'>{formattedDate}</span></p>
                          <p style='margin: 8px 0; font-size: 1rem; line-height: 1.5; color: #555555; padding: 10px; background-color: #f9f9f9; border-radius: 5px;'>Thời gian: <span style='font-weight: bold; color: #333333;'>{formattedTime}</span></p>
                          <p style='font-size: 1.2rem; font-weight: bold; margin-bottom: 10px; color: #333333;'>Danh sách phòng và bàn đã đặt:</p>
                          <div style='margin: 10px 0; font-size: 1rem; line-height: 1.5; color: #555555; padding: 10px; background-color: #f9f9f9; border-radius: 5px;'>
                            {tablesHtml}
                          </div>
                        </div>

                        <div style='background-color: #f29034; padding: 20px; color: #ffffff; text-align: center;'>
                          <h1 style='margin: 0;'>Tổng hóa đơn</h1>
                          <div style='font-size: 1.2rem; font-weight: bold; margin-top: 10px;'>{orderConfirmationEmail.TotalAmount.ToString("N0")} đ</div>
                          <div style='margin: 20px auto; width: 120px; height: 120px;'>
                            <img src='https://api.qrserver.com/v1/create-qr-code/?size=150x150&data=OrderID{orderConfirmationEmail.OrderId}' alt='QR Code' style='width: 100%; height: 100%;' />
                          </div>
                          <p>Vui lòng mang theo mã QR này khi đến nhà hàng.</p>
                          <p>Chúc bạn có một bữa ăn ngon miệng!</p>
                        </div>
                      </div>
                    </body>
                    </html>";
        }



        //public static string Order(OrderConfirmationEmail orderConfirmationEmail)
        //{
        //    string formattedDate = orderConfirmationEmail.ReservationDate.ToString("dd/MM/yyyy");
        //    string formattedTime = orderConfirmationEmail.ReservationTime.ToString(@"hh\:mm");

        //    var tablesHtml = "<ul>";
        //    foreach (var table in orderConfirmationEmail.TableName)
        //    {
        //        tablesHtml += $"<li>{table}</li>";
        //    }
        //    tablesHtml += "</ul>";

        //    return $@"
        //    <!DOCTYPE html>
        //    <html lang='vi'>
        //    <head>
        //      <meta charset='UTF-8'>
        //      <meta name='viewport' content='width=device-width, initial-scale=1.0'>
        //    </head>
        //    <body style='font-family: Arial, sans-serif; background-color: #f4f4f4; margin: 0; padding: 0;'>
        //      <div style='width: 100%; max-width: 600px; margin: 20px auto; background-color: #ffffff; border-radius: 10px; box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1); overflow: hidden;'>
        //        <div style='background-color: #f29034; color: #ffffff; text-align: center; padding: 20px; font-size: 1.5rem; font-weight: bold;'>
        //          Xác nhận đặt bàn thành công
        //        </div>

        //        <div style='padding: 20px;'>
        //          <img src='https://res.cloudinary.com/do9iyczi3/image/upload/v1726643328/LOGO-TOPDER_qonl9l.png' alt='Logo TOPDER' style='display: block; margin: 20px auto; width: 120px;' />
        //          <p style='color: #f29034; font-weight: bold; font-size: 1.1rem; margin: 15px 0;'>Cảm ơn bạn đã đặt bàn tại TOPDER!</p>
        //          <p style='font-size: 1.2rem; font-weight: bold; margin-bottom: 10px; color: #333333;'>Thông tin đặt bàn của bạn như sau:</p>
        //          <p style='margin: 8px 0; font-size: 1rem; line-height: 1.5; color: #555555; padding: 10px; background-color: #f9f9f9; border-radius: 5px;'>Mã đơn hàng: <span style='font-weight: bold; color: #333333;'>#{orderConfirmationEmail.OrderId}</span></p>
        //          <p style='margin: 8px 0; font-size: 1rem; line-height: 1.5; color: #555555; padding: 10px; background-color: #f9f9f9; border-radius: 5px;'>Tên khách hàng: <span style='font-weight: bold; color: #333333;'>{orderConfirmationEmail.Name}</span></p>
        //          <p style='margin: 8px 0; font-size: 1rem; line-height: 1.5; color: #555555; padding: 10px; background-color: #f9f9f9; border-radius: 5px;'>Nhà hàng: <span style='font-weight: bold; color: #333333;'>{orderConfirmationEmail.RestaurantName}</span></p>
        //          <p style='margin: 8px 0; font-size: 1rem; line-height: 1.5; color: #555555; padding: 10px; background-color: #f9f9f9; border-radius: 5px;'>Số lượng khách: <span style='font-weight: bold; color: #333333;'>{orderConfirmationEmail.NumberOfGuests} người</span></p>
        //          <p style='margin: 8px 0; font-size: 1rem; line-height: 1.5; color: #555555; padding: 10px; background-color: #f9f9f9; border-radius: 5px;'>Ngày đặt bàn: <span style='font-weight: bold; color: #333333;'>{formattedDate}</span></p>
        //          <p style='margin: 8px 0; font-size: 1rem; line-height: 1.5; color: #555555; padding: 10px; background-color: #f9f9f9; border-radius: 5px;'>Thời gian: <span style='font-weight: bold; color: #333333;'>{formattedTime}</span></p>
        //          <p style='font-size: 1.2rem; font-weight: bold; margin-bottom: 10px; color: #333333;'>Danh sách bàn đã đặt:</p>
        //          <div style='margin: 10px 0; font-size: 1rem; line-height: 1.5; color: #555555; padding: 10px; background-color: #f9f9f9; border-radius: 5px;'>
        //            {tablesHtml}
        //          </div>
        //        </div>

        //        <div style='background-color: #f29034; padding: 20px; color: #ffffff; text-align: center;'>
        //          <h1 style='margin: 0;'>Tổng hóa đơn</h1>
        //          <div style='font-size: 1.2rem; font-weight: bold; margin-top: 10px;'>{orderConfirmationEmail.TotalAmount.ToString("N0")} đ</div>
        //          <div style='margin: 20px auto; width: 120px; height: 120px;'>
        //            <img src='https://api.qrserver.com/v1/create-qr-code/?size=150x150&data=OrderID{orderConfirmationEmail.OrderId}' alt='QR Code' style='width: 100%; height: 100%;' />
        //          </div>
        //          <p>Vui lòng mang theo mã QR này khi đến nhà hàng.</p>
        //          <p>Chúc bạn có một bữa ăn ngon miệng!</p>
        //        </div>
        //      </div>
        //    </body>
        //    </html>";
        //}


    }
}
