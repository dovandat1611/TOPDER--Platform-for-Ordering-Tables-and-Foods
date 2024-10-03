using AutoMapper;
using MailKit.Search;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Repository.Entities;
using TOPDER.Repository.IRepositories;
using TOPDER.Repository.Repositories;
using TOPDER.Service.Dtos.Discount;
using TOPDER.Service.Dtos.Email;
using TOPDER.Service.Dtos.Menu;
using TOPDER.Service.Dtos.Order;
using TOPDER.Service.Dtos.RestaurantRoom;
using TOPDER.Service.IServices;
using TOPDER.Service.Utils;
using static TOPDER.Service.Common.ServiceDefinitions.Constants;

namespace TOPDER.Service.Services
{
    public class OrderService : IOrderService
    {
        private readonly IMapper _mapper;
        private readonly IOrderRepository _orderRepository;


        public OrderService(IOrderRepository orderRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        public async Task<Order> AddAsync(OrderDto orderDto)
        {
            var order = _mapper.Map<Order>(orderDto);
            return await _orderRepository.CreateAndReturnAsync(order);
        }

        public async Task<bool> CheckIsFirstOrderAsync(int customerId, int restaurantId)
        {
            var queryable = await _orderRepository.QueryableAsync();
            var isFirstOrder = !await queryable.AnyAsync(x => x.CustomerId == customerId && x.RestaurantId == restaurantId);
            return isFirstOrder;
        }


        public async Task<PaginatedList<OrderCustomerDto>> GetCustomerPagingAsync(int pageNumber, int pageSize, int customerId)
        {
            var queryable = await _orderRepository.QueryableAsync();

            var query = queryable.Where(x => x.CustomerId == customerId);

            var queryDTO = query.Select(r => _mapper.Map<OrderCustomerDto>(r));

            var paginatedDTOs = await PaginatedList<OrderCustomerDto>.CreateAsync(
                queryDTO.AsNoTracking(),
                pageNumber > 0 ? pageNumber : 1,
                pageSize > 0 ? pageSize : 10
            );
            return paginatedDTOs;
        }

        public async Task<EmailForOrder> GetEmailForOrderAsync(int orderId, string role)
        {
            var queryable = await _orderRepository.QueryableAsync();
            var order = await queryable
                .Include(x => x.Customer) // Luôn bao gồm Customer
                .Include(x => x.Restaurant) // Luôn bao gồm Restaurant
                .FirstOrDefaultAsync(x => x.OrderId == orderId);

            if (order == null)
            {
                throw new KeyNotFoundException($"Đơn hàng với ID {orderId} không tồn tại.");
            }

            // Khai báo biến để lưu thông tin email và tên
            string email = string.Empty;
            string name = string.Empty;

            // Kiểm tra vai trò và gán giá trị tương ứng
            if (role.Equals(User_Role.CUSTOMER))
            {
                if (order.Customer != null && order.Customer.UidNavigation != null)
                {
                    email = order.Customer.UidNavigation.Email;
                    name = order.Customer.Name;
                }
                else
                {
                    throw new KeyNotFoundException($"Đơn hàng với ID {orderId} không có thông tin khách hàng.");
                }
            }
            else if (role.Equals(User_Role.RESTAURANT))
            {
                if (order.Restaurant != null && order.Restaurant.UidNavigation != null)
                {
                    email = order.Restaurant.UidNavigation.Email;
                    name = order.Restaurant.NameRes;
                }
                else
                {
                    throw new KeyNotFoundException($"Đơn hàng với ID {orderId} không có thông tin nhà hàng.");
                }
            }

            // Tạo đối tượng OrderChangeStatusEmail
            var orderChangeStatusEmail = new EmailForOrder()
            {
                OrderId = orderId.ToString(),
                Email = email,
                Name = name,
            };

            // Kiểm tra Email và Name không null
            if (string.IsNullOrEmpty(orderChangeStatusEmail.Email) || string.IsNullOrEmpty(orderChangeStatusEmail.Name))
            {
                throw new InvalidOperationException($"Không có thông tin email hoặc tên cho đơn hàng với ID {orderId}.");
            }

            return orderChangeStatusEmail;
        }




        public async Task<OrderDto> GetItemAsync(int id, int Uid)
        {
            var order = await _orderRepository.GetByIdAsync(id);

            if (order == null)
            {
                throw new KeyNotFoundException($"Order với id {id} không tồn tại.");
            }

            if (order.RestaurantId != Uid && order.CustomerId != Uid)
            {
                throw new UnauthorizedAccessException($"Order với id {id} không thuộc user với id {Uid}.");
            }
            return _mapper.Map<OrderDto>(order);
        }

        public async Task<OrderPaidEmail> GetOrderPaid(int orderID)
        {
            // Bước 1: Lấy truy vấn ban đầu từ repository
            var queryOrder = await _orderRepository.QueryableAsync();

            // Bước 2: Thêm các Include vào truy vấn
            queryOrder = queryOrder
                .Include(x => x.OrderTables)
                    .ThenInclude(ot => ot.Table)
                        .ThenInclude(t => t.Room)
                .Include(x => x.Restaurant)
                .Include(x => x.Customer)
                    .ThenInclude(c => c.UidNavigation);

            // Bước 3: Thực hiện truy vấn FirstOrDefault
            var order = await queryOrder.FirstOrDefaultAsync(x => x.OrderId == orderID);

            // Kiểm tra nếu order không tồn tại
            if (order == null)
            {
                throw new Exception("Order not found");
            }

            // Khởi tạo email
            OrderPaidEmail orderPaidEmail = new OrderPaidEmail()
            {
                Name = order.Customer.Name,
                Email = order.Customer.UidNavigation.Email,
                RestaurantName = order.Restaurant.NameRes,
                OrderId = order.OrderId.ToString(),
                NumberOfGuests = order.NumberChild + order.NumberPerson,
                ReservationDate = order.DateReservation,
                ReservationTime = order.TimeReservation,
                TotalAmount = order.TotalAmount,
                Rooms = new List<RoomEmail>(),
                TableName = new List<string>()
            };

            // Xử lý các bàn và phòng
            foreach (var orderTable in order.OrderTables)
            {
                var table = orderTable.Table;
                var room = table?.Room;

                if (room != null)
                {
                    var existingRoom = orderPaidEmail.Rooms.FirstOrDefault(r => r.RoomName == room.RoomName);
                    if (existingRoom != null)
                    {
                        existingRoom.Tables.Add(table.TableName);
                    }
                    else
                    {
                        var roomEmail = new RoomEmail
                        {
                            RoomName = room.RoomName,
                            Tables = new List<string> { table.TableName }
                        };
                        orderPaidEmail.Rooms.Add(roomEmail);
                    }
                }
                else
                {
                    orderPaidEmail.TableName.Add(table.TableName);
                }
            }

            return orderPaidEmail;
        }


        public async Task<PaginatedList<OrderDto>> GetRestaurantPagingAsync(int pageNumber, int pageSize, int restaurantId)
        {
            var queryable = await _orderRepository.QueryableAsync();

            var query = queryable.Where(x => x.RestaurantId == restaurantId);

            var queryDTO = query.Select(r => _mapper.Map<OrderDto>(r));

            var paginatedDTOs = await PaginatedList<OrderDto>.CreateAsync(
                queryDTO.AsNoTracking(),
                pageNumber > 0 ? pageNumber : 1,
                pageSize > 0 ? pageSize : 10
            );

            return paginatedDTOs;
        }

        public Task<bool> RemoveAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateAsync(OrderDto orderDto)
        {
            var existingOrder = await _orderRepository.GetByIdAsync(orderDto.OrderId);
            if (existingOrder == null)
            {
                return false;
            }
            var order = _mapper.Map<Order>(orderDto);
            return await _orderRepository.UpdateAsync(order);
        }

        public async Task<bool> UpdateStatusAsync(int orderID, string status)
        {
            var order = await _orderRepository.GetByIdAsync(orderID);

            if (order == null)
            {
                return false;
            }

            if (!string.IsNullOrEmpty(order.StatusOrder))
            {
                if (order.StatusOrder.Equals(status))
                {
                    return false;
                }
                order.StatusOrder = status;
                return await _orderRepository.UpdateAsync(order);
            }
            return false;
        }


        public async Task<bool> UpdateStatusOrderPayment(int orderID, string status)
        {
            var order = await _orderRepository.GetByIdAsync(orderID);

            if (order == null)
            {
                return false; 
            }

            if (!string.IsNullOrEmpty(order.StatusPayment))
            {
                if (order.StatusPayment.Equals(status))
                {
                    return false; 
                }

                if (status.Equals(Payment_Status.SUCCESSFUL))
                {
                    order.StatusPayment = status;
                    order.StatusOrder = Order_Status.PAID;
                }
                else
                {
                    order.StatusPayment = status; 
                }
                return await _orderRepository.UpdateAsync(order);
            }
            return false; 
        }

    }
}
