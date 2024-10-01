using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TOPDER.Service.Dtos.OrderMenu;
using TOPDER.Service.IServices;

namespace TOPDER.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderMenuController : ControllerBase
    {
        private readonly IOrderMenuService _orderMenuService;

        public OrderMenuController(IOrderMenuService orderMenuService)
        {
            _orderMenuService = orderMenuService;
        }

        /// <summary>
        /// Adds a list of order menus.
        /// </summary>
        /// <param name="orderMenuDtos">The order menu DTOs to be added.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating success.</returns>
        [HttpPost]
        public async Task<IActionResult> AddOrderMenus([FromBody] List<CreateOrUpdateOrderMenuDto> orderMenuDtos)
        {
            if (orderMenuDtos == null || !orderMenuDtos.Any())
            {
                return BadRequest("Order menu list cannot be null or empty.");
            }

            var result = await _orderMenuService.AddAsync(orderMenuDtos);
            if (result)
            {
                return CreatedAtAction(nameof(GetItemsByOrderAsync), new { id = orderMenuDtos.First().OrderId }, orderMenuDtos);
            }

            return StatusCode(500, "An error occurred while adding order menus.");
        }

        /// <summary>
        /// Gets items by order ID.
        /// </summary>
        /// <param name="id">The order ID.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of order menu DTOs.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetItemsByOrderAsync(int id)
        {
            var items = await _orderMenuService.GetItemsByOrderAsync(id);
            if (items == null || !items.Any())
            {
                return NotFound("No items found for the specified order ID.");
            }

            return Ok(items);
        }

    }
}
