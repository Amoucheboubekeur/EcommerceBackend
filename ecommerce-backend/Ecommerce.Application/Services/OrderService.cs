using Ecommerce.Application.Common.Pagination;
using Ecommerce.Application.Interfaces;
using Ecommerce.Domain.DTOs.Orders;
using Ecommerce.Domain.Entities;
using Ecommerce.Domain.Interfaces;

namespace Ecommerce.Application.Services
{
    public class OrderService(IOrderRepository orderRepository, IProductRepository productRepository, IUserRepository userRepository) : IOrderService
    {
        private readonly IOrderRepository _orderRepository = orderRepository;
        private readonly IProductRepository _productRepository = productRepository;
        private readonly IUserRepository _userRepository = userRepository;


        public async Task<Guid> CreateGuestOrderAsync(GuestOrderDTO dto)
        {
            try
            {
                // 🔹 1. Vérifier si un utilisateur invité existe déjà
                var existingUser = await _userRepository.FindByEmailOrPhoneAsync(dto.Email, dto.Phone);
                ApplicationUser user;

                if (existingUser != null)
                {
                    user = existingUser;
                }
                else
                {
                    // 🔹 2. Créer un utilisateur invité
                    user = new ApplicationUser
                    {
                        UserName = $"{dto.FirstName}_{dto.LastName}_{Guid.NewGuid():N}".Substring(0, 20),
                        Email = string.IsNullOrEmpty(dto.Email) ? "guest@guest.com" : dto.Email,
                        PhoneNumber = dto.Phone,
                        Address = dto.Address,
                        FirstName = dto.FirstName,
                        LastName = dto.LastName,
                        Wilaya = dto.Wilaya,
                        Commune = dto.Commune,
                        IsGuest = true
                    };

                    await _userRepository.AddAsync(user, "Guest@123");
                }

                // 🔹 3. Construire le DTO de création de commande (sans total)
                var createOrder = new OrderCreateDto
                {
                    ShippingAddress = dto.Address,
                    Wilaya = dto.Wilaya,
                    Commune = dto.Commune,
                    DeliveryMethod = dto.DeliveryMethod,
                    DeliveryPrice = dto.DeliveryPrice,
                    Items =
                    [
                        new OrderItemCreateDto
                {
                    ProductId = dto.ProductId,
                    Quantity = dto.Quantity,
                    VariantId = !string.IsNullOrEmpty(dto.VariantId) ? Guid.Parse(dto.VariantId) : null
                }
                    ]
                };

                // 🔹 4. Appel de la création d’une commande réelle
                return await CreateOrderAsync(createOrder, user.Id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur CreateGuestOrderAsync : {ex.Message}");
                throw;
            }
        }

        public async Task UpdateOrderStatusAsync(Guid orderId, string newStatus)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
                throw new KeyNotFoundException("Commande introuvable.");

            order.UpdateStatus(newStatus);

            await _orderRepository.UpdateAsync(order);
        }

        public async Task<Guid> CreateOrderAsync(OrderCreateDto dto, string userId)
        {
            var order = new Order
            {
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                Status = "Pending",
                ShippingAddress = dto.ShippingAddress,
                Wilaya = dto.Wilaya,
                Commune = dto.Commune,
                DeliveryPrice = dto.DeliveryPrice,
                DeliveryMethod = Enum.TryParse<DeliveryMethod>(dto.DeliveryMethod, true, out var method)
                    ? method
                    : DeliveryMethod.Maison
            };

            decimal total = 0m;

            foreach (var itemDto in dto.Items)
            {
                var product = await _productRepository.GetByIdAsync(itemDto.ProductId)
                    ?? throw new Exception($"Produit introuvable : {itemDto.ProductId}");

                decimal unitPrice = product.FinalPrice > 0 ? product.FinalPrice : product.Price;

                string? nameVariant = "";
                decimal? additionalPriceVariant = 0;

                // 🔸 Gestion de la variante
                if (itemDto.VariantId.HasValue)
                {
                    var variant = product.Variants.FirstOrDefault(v => v.Id == itemDto.VariantId);
                    if (variant != null)
                    {
                        nameVariant = variant.Name;
                        additionalPriceVariant = variant.AdditionalPrice;
                        unitPrice += variant.AdditionalPrice ?? 0;

                        // 🔹 Mise à jour du stock de la variante (si géré séparément)
                        if (variant.VariantStock < itemDto.Quantity)
                            throw new Exception($"Stock insuffisant pour la variante {variant.Name} du produit {product.Title}");

                        variant.UpdateStock(itemDto.Quantity);
                    }
                }
                else
                {
                    // 🔹 Mise à jour du stock du produit de base
                    if (product.Stock < itemDto.Quantity)
                        throw new Exception($"Stock insuffisant pour le produit {product.Title}");

                    
                }
                product.UpdateStock(itemDto.Quantity);

                // 🔹 Création de l’item de commande
                var orderItem = new OrderItem
                {
                    ProductId = product.Id,
                    Quantity = itemDto.Quantity,
                    UnitPrice = unitPrice,
                    NameVariant = nameVariant,
                    AdditionalPriceVariant = additionalPriceVariant
                };

                order.Items.Add(orderItem);

                // 🔹 Ajout au total
                total += unitPrice * itemDto.Quantity;

                // ✅ Marquer le produit comme modifié pour EF
                await _productRepository.UpdateAsync(product);
            }

            total += dto.DeliveryPrice;
            order.Total = total;

            // ✅ Sauvegarder la commande (et les produits mis à jour)
            var orderId = await _orderRepository.CreateAsync(order);

            return orderId;
        }




        public async Task<PagedResult<Order>> GetPagedOrdersAsync(PaginationParams pagination)
        {
            var query = _orderRepository.GetAllOrdersWithDetails();

            // 🔍 Filtrage
            if (!string.IsNullOrWhiteSpace(pagination.Search))
            {
                query = query.Where(o =>
                    (o.User.FirstName + " " + o.User.LastName).Contains(pagination.Search) ||
                    o.User.Email.Contains(pagination.Search) ||
                    o.User.Wilaya.Contains(pagination.Search));
            }

            if (!string.IsNullOrEmpty(pagination.Status) && pagination.Status != "all")
            {
                query = query.Where(o => o.Status.ToLower() == pagination.Status.ToLower());
            }

            // 🔽 Tri
            query = pagination.Sort == "asc"
                ? query.OrderBy(o => o.CreatedAt)
                : query.OrderByDescending(o => o.CreatedAt);

            // 📄 Pagination
            return await query.ToPagedResultAsync(pagination.Page, pagination.PageSize);
        }
        // ✅ Créer une commande
      

        // ✅ Récupérer une commande par ID
        public async Task<OrderResponseDto> GetOrderByIdAsync(Guid orderId, string userId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId, userId);
            return order == null ? throw new Exception("Order not found") : MapOrder(order);
        }
        public async Task<List<Order>> Getallorderasync()
        {
            var orders = await _orderRepository.Getallorderasync();
            return  orders;
        }

        // ✅ Récupérer toutes les commandes d’un utilisateur
        public async Task<List<OrderResponseDto>> GetUserOrdersAsync(string userId)
        {
            var orders = await _orderRepository.GetAllByUserIdAsync(userId);
            return [.. orders.Select(MapOrder)];
        }

        // ✅ Mettre à jour une commande
        public async Task<bool> UpdateOrderAsync(Guid orderId, OrderUpdateDto dto, string userId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId, userId);
            if (order == null || order.Status == "Confirmed")
                return false;

            if (!string.IsNullOrEmpty(dto.ShippingAddress))
                order.ShippingAddress = dto.ShippingAddress;

            if (dto.Items != null && dto.Items.Count != 0)
            {
                order.Items.Clear();

                foreach (var itemDto in dto.Items)
                {
                    var product = await _productRepository.GetByIdAsync(itemDto.ProductId) ?? throw new Exception($"Product {itemDto.ProductId} not found");
                    var orderItem = new OrderItem
                    {
                        ProductId = product.Id,
                        Quantity = itemDto.Quantity,
                        UnitPrice = product.Price
                    };

                    order.Items.Add(orderItem);
                }

                order.Total = order.Items.Sum(i => i.UnitPrice * i.Quantity);
            }

            await _orderRepository.UpdateAsync(order);
            return true;
        }

        // ✅ Supprimer une commande
        public async Task<bool> DeleteOrderAsync(Guid orderId, string userId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId, userId);
            if (order == null || order.Status == "Confirmed")
                return false;

            await _orderRepository.DeleteAsync(order);
            return true;
        }

        // ✅ Confirmer une commande
     public async Task<bool> ConfirmOrderAsync(Guid orderId, string userId)
{
    // 🔹 1. Récupérer la commande avec ses items
    var order = await _orderRepository.GetByIdAsync(orderId, userId);
    if (order == null || order.Status == "Confirmed")
        return false;

    // 🔹 2. Mettre à jour le statut
    order.Status = "Confirmed";
    order.ConfirmedAt = DateTime.UtcNow;

    // 🔹 3. Incrémenter le SalesCount pour chaque produit
    foreach (var item in order.Items)
    {
        var product = await _productRepository.GetByIdAsync(item.ProductId);
        if (product != null)
        {
           // product.SalesCount += item.Quantity; // ⚡ ajoute le nombre vendu
            await _productRepository.UpdateAsync(product);
        }
    }

    // 🔹 4. Sauvegarder la commande mise à jour
    await _orderRepository.UpdateAsync(order);

    return true;
}


        // ✅ Mapping interne
        private static OrderResponseDto MapOrder(Order order)
        {
            return new OrderResponseDto
            {
                Id = order.Id,
                Total = (decimal)order.Total,
                Status = order.Status,
                CreatedAt = order.CreatedAt,
                ConfirmedAt = order.ConfirmedAt,
                ShippingAddress = order.ShippingAddress, 
                
                Items = order.Items?.Select(i => new OrderItemResponseDto
                {
                    ProductId = i.ProductId,
                    ProductTitle = i.Product?.Title ?? "",
                    Quantity = i.Quantity,
                    Price = i.UnitPrice
                }).ToList() ?? []
            };
        }

        public async Task<PagedResultDto<Order>> GetPagedOrdersAsync(int page, int pageSize, string? search, string? status)
        {
            var (orders, totalCount) = await _orderRepository.GetPagedAsync(page, pageSize, search, status);

            return new PagedResultDto<Order>
            {
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                CurrentPage = page,
                PageSize = pageSize,
                Data = orders
            };
        }
    }
}
