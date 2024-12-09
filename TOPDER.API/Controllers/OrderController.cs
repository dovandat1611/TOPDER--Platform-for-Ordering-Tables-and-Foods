using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TOPDER.Service.Dtos.Order;
using TOPDER.Service.IServices;

namespace TOPDER.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        /// <summary>
        /// Adds a new order.
        /// </summary>
        /// <param name="orderDto">The order DTO to be added.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the created order ID.</returns>
        [HttpPost]
        public async Task<IActionResult> AddOrder([FromBody] OrderDto orderDto)
        {
            if (orderDto == null)
            {
                return BadRequest("Order cannot be null.");
            }

            var result = await _orderService.AddAsync(orderDto);
            if (result)
            {
                return BadRequest("Tạo Order thành công.");
            }
            return StatusCode(500, "An error occurred while adding the order.");
        }

        /// <summary>
        /// Gets an order by ID.
        /// </summary>
        /// <param name="id">The order ID.</param>
        /// <param name="Uid">The user ID.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the order DTO.</returns>
        //[HttpGet("{id}")]
        //public async Task<IActionResult> GetItemAsync(int id, int Uid)
        //{
        //    try
        //    {
        //        var orderDto = await _orderService.GetItemAsync(id, Uid);
        //        return Ok(orderDto);
        //    }
        //    catch (KeyNotFoundException)
        //    {
        //        return NotFound($"Order with id {id} not found.");
        //    }
        //    catch (UnauthorizedAccessException)
        //    {
        //        return Forbid($"You do not have access to order with id {id}.");
        //    }
        //}

        /// <summary>
        /// Gets paginated orders for a specific customer.
        /// </summary>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="customerId">The customer ID.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the paginated list of order DTOs.</returns>
        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetCustomerPaging(int pageNumber, int pageSize, int customerId)
        {
            var paginatedOrders = await _orderService.GetCustomerPagingAsync(pageNumber, pageSize, customerId);
            return Ok(paginatedOrders);
        }

        /// <summary>
        /// Gets paginated orders for a specific restaurant.
        /// </summary>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="restaurantId">The restaurant ID.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the paginated list of order DTOs.</returns>
        [HttpGet("restaurant/{restaurantId}")]
        public async Task<IActionResult> GetRestaurantPaging(int pageNumber, int pageSize, int restaurantId)
        {
            var paginatedOrders = await _orderService.GetRestaurantPagingAsync(pageNumber, pageSize, restaurantId);
            return Ok(paginatedOrders);
        }

        /// <summary>
        /// Updates an existing order.
        /// </summary>
        /// <param name="orderDto">The order DTO with updated information.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an action result indicating success or failure.</returns>
      /*  [HttpPut]
        public async Task<IActionResult> UpdateOrder([FromBody] OrderDto orderDto)
        {
            if (orderDto == null)
            {
                return BadRequest("Order cannot be null.");
            }

            var result = await _orderService.UpdateAsync(orderDto);
            if (result)
            {
                return NoContent(); // Success, but no content to return
            }

            return NotFound($"Order with ID {orderDto.OrderId} not found.");
        }*/

        /// <summary>
        /// Deletes an order by ID.
        /// </summary>
        /// <param name="id">The order ID.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an action result indicating success or failure.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveOrder(int id)
        {
            // Implement the remove functionality
            // Assuming RemoveAsync is implemented in your service
            var result = await _orderService.RemoveAsync(id);
            if (result)
            {
                return NoContent(); // Success
            }

            return NotFound($"Order with ID {id} not found.");
        }
    }
}
