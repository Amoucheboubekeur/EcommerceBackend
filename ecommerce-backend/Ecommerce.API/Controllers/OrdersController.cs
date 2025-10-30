using Ecommerce.Application.Common.Pagination;
using Ecommerce.Application.Interfaces;
using Ecommerce.Domain.DTOs.Orders;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Ecommerce.API.Controllers
{
   [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "SuperAdmin,Admin")] // Seul le SuperAdmin peut accéder ici
    public class OrdersController(IOrderService orderService) : ControllerBase
    {
        private readonly IOrderService _orderService = orderService;
        [HttpPost("guest-order")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateGuestOrder([FromBody] GuestOrderDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var orderId = await _orderService.CreateGuestOrderAsync(dto);

            if (orderId == Guid.Empty)
                return StatusCode(500, "Erreur lors de la création de la commande.");

            return Ok(new { Message = "Commande enregistrée avec succès ✅", OrderId = orderId });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] OrderCreateDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "guest";

            var orderId = await _orderService.CreateOrderAsync(dto, userId);

            return CreatedAtAction(nameof(GetById), new { id = orderId }, new { orderId });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)??"guest";
            var order = await _orderService.GetOrderByIdAsync(id, userId);
            if (order == null) return NotFound();
            return Ok(order);
        }


        [HttpPut("{orderId:guid}/status")]
        public async Task<IActionResult> UpdateOrderStatus(Guid orderId, [FromBody] UpdateOrderStatusDto dto)
        {
            try
            {
                await _orderService.UpdateOrderStatusAsync(orderId, dto.Status);
                return Ok(new { message = "Statut mis à jour avec succès", status = dto.Status });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Commande introuvable" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "guest";
            var orders = await _orderService.GetUserOrdersAsync(userId);
            return Ok(orders);
        }

        [HttpGet("GetAllforAllUsers")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> GetAllforAllUsers()
        {
            var orders = await _orderService.Getallorderasync();
            return Ok(orders);
        }


        [HttpGet("GetPaged")]
         [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> GetPaged([FromQuery] PaginationParams pagination)
        {
            var result = await _orderService.GetPagedOrdersAsync(pagination);

            var dto = result.Data.Select(o => new {
                o.Id,
                User = new
                {
                    o.User.FirstName,
                    o.User.LastName,
                    o.User.Email,
                    o.User.Address,
                    o.User.Wilaya,
                    o.User.PhoneNumber
                },
                o.Wilaya,
                o.Commune,
                o.DeliveryMethod,
                o.DeliveryPrice,
                o.ShippingAddress,
                o.Total,
                o.Status,
                o.CreatedAt,
                Items = o.Items.Select(i => new {
                    Product = new
                    {
                        i.Product.Title,
                        i.Product.Price
                    },
                    i.Quantity,
                    i.UnitPrice
                })
            });

            return Ok(new
            {
                result.TotalPages,
                result.PageSize,
                result.CurrentPage,
                result.TotalCount,
                Data = dto
            });
        }





        //[HttpGet("GetPaged")]
        ////public async Task<IActionResult> GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null, [FromQuery] string? status = null)
        ////{
        ////    var result = await _orderService.GetPagedOrdersAsync(page, pageSize, search, status);
        ////    return Ok(result);
        ////}
        /// <summary>
        /// Modifier une commande (ex: adresse, items, etc.)
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] OrderUpdateDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "guest";
            var updated = await _orderService.UpdateOrderAsync(id, dto, userId);
            if (!updated) return NotFound("Commande non trouvée ou non modifiable.");
            return NoContent();
        }

        /// <summary>
        /// Supprimer une commande
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "guest";
            var deleted = await _orderService.DeleteOrderAsync(id, userId);
            if (!deleted) return NotFound("Commande non trouvée ou déjà supprimée.");
            return NoContent();
        }

        /// <summary>
        /// Confirmer une commande (passer son statut à Confirmed)
        /// </summary>
        [HttpPut("{id}/confirm")]
        public async Task<IActionResult> Confirm(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "guest";
            var confirmed = await _orderService.ConfirmOrderAsync(id, userId);
            if (!confirmed) return BadRequest("Impossible de confirmer cette commande.");
            return Ok(new { message = "Commande confirmée avec succès." });
        }
    }
}
