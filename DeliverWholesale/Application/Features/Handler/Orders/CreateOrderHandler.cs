using DeliverWholesale.Domain.Entities;
using DeliverWholesale.Domain.Enums;
using DeliverWholesale.Infrastructure.Data;
using DeliverWholesale.Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DeliverWholesale.Application.Features.Handler.Orders
{
    public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, int>
    {
        private readonly ApplicationDbContext _context;
        private readonly NotificationService _notification;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IInventoryService _inventory;

        public CreateOrderHandler(
            ApplicationDbContext context,
            NotificationService notification,
            IHttpContextAccessor httpContextAccessor,
            IInventoryService inventory)
        {
            _context = context;
            _notification = notification;
            _httpContextAccessor = httpContextAccessor;
            _inventory = inventory;
        }

        public async Task<int> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            if (request.OrderDto.Items == null || !request.OrderDto.Items.Any())
                throw new ApplicationException("Aucun produit dans la commande.");

            // ✅ Récupérer l'utilisateur depuis le JWT
            var userIdClaim = _httpContextAccessor.HttpContext?.User
                .FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdClaim))
                throw new ApplicationException("Utilisateur non authentifié.");

            var userId = int.Parse(userIdClaim);

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken)
                ?? throw new ApplicationException("Utilisateur introuvable.");

            try
            {
                var listProduitId = request.OrderDto.Items.Select(x => x.ProduitId).ToList();

                // ✅ CORRIGÉ : On charge les produits AVEC leurs prix
                var products = await _context.Produits
                    .Include(x => x.PrixVentes)
                    .Where(x => listProduitId.Contains(x.idP) && x.IsActive)
                    .ToListAsync(cancellationToken);

                if (products.Count != listProduitId.Count)
                    throw new ApplicationException("Un ou plusieurs produits sont invalides ou inactifs.");

                var order = new Order
                {
                    DateCommande = DateTime.UtcNow,
                    FraisLivraison = 0,
                    Statut = StatutOrder.EnAttente,
                    User = user,
                };

                var orderItems = new List<OrderDetail>();
                decimal totalProduits = 0;

                // ✅ Création des OrderDetails et calcul du total
                foreach (var item in request.OrderDto.Items)
                {
                    var product = products.First(p => p.idP == item.ProduitId);
                    var prixUnitaire = product.PrixVenteActuel ?? 0;

                    var detail = new OrderDetail
                    {
                        Order = order,
                        PrixUnitaire = prixUnitaire,
                        Produit = product,
                        Quantite = item.Quantite
                    };

                    orderItems.Add(detail);
                    totalProduits += detail.SousTotal;
                }

                order.TotalProduits = totalProduits;
                _context.Orders.Add(order);
                _context.OrderDetails.AddRange(orderItems);

                // Persist order and details first to obtain IDs for allocation
                await _context.SaveChangesAsync(cancellationToken);

                // Prepare allocation items (ProduitId -> OrderDetailId -> Quantite)
                var allocateItems = orderItems.Select(od => new AllocateItem(od.ProduitId, od.Id, od.Quantite)).ToList();

                try
                {
                    // Call centralized inventory allocation. This will create LotCommande and Transaction rows,
                    // and update StockLot.QuantiteRestante atomically. If it fails it will throw and no inventory changes are persisted.
                    var allocations = await _inventory.AllocateOrderStockAsync(order.Id, allocateItems);
                }
                catch (Exception)
                {
                    // Allocation failed: remove created order/details to rollback the whole operation
                    _context.OrderDetails.RemoveRange(orderItems);
                    _context.Orders.Remove(order);
                    await _context.SaveChangesAsync(cancellationToken);
                    throw;
                }

                await _notification.NotifyAdmins(
                    $"Nouvelle commande créée (ID: {order.Id}) par {user.Prenom} {user.Nom}",
                    "CREATE_ORDER"
                );
                return order.Id;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}