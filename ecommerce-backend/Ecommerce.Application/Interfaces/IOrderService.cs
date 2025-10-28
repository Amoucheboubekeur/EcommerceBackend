using Ecommerce.Application.Common.Pagination;
using Ecommerce.Domain.DTOs.Orders;
using Ecommerce.Domain.Entities;

namespace Ecommerce.Application.Interfaces;

public interface IOrderService
    {
        Task<Guid> CreateOrderAsync(OrderCreateDto dto, string userId);
        Task<OrderResponseDto> GetOrderByIdAsync(Guid orderId, string userId);
        Task<List<OrderResponseDto>> GetUserOrdersAsync(string userId);
        Task<bool> UpdateOrderAsync(Guid id, OrderUpdateDto dto, string userId);
        Task<bool> DeleteOrderAsync(Guid id, string userId);
        Task<bool> ConfirmOrderAsync(Guid id, string userId);
        Task<Guid> CreateGuestOrderAsync(GuestOrderDTO dto);
        Task<List<Order>> Getallorderasync();
        Task<PagedResultDto<Order>> GetPagedOrdersAsync(int page, int pageSize, string? search, string? status);

    Task<PagedResult<Order>> GetPagedOrdersAsync(PaginationParams pagination);
    Task UpdateOrderStatusAsync(Guid orderId, string newStatus);
}

