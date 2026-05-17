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

        public CreateOrderHandler(
            ApplicationDbContext context,
            NotificationService notification,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _notification = notification;
            _httpContextAccessor = httpContextAccessor;
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

            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

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

                // ✅ CORRIGÉ : Récupération du stock de manière sécurisée
                var stockLots = await _context.StockLots
                    .Where(x => listProduitId.Contains(x.ProduitId) && x.QuantiteRestante > 0)
                    .OrderBy(x => x.DateReception) // FIFO : on vide les vieux stocks d'abord
                    .ToListAsync(cancellationToken);

                var listTransation = new List<Transaction>();
                var itemsToProcess = request.OrderDto.Items.ToDictionary(i => i.ProduitId, i => i.Quantite);

                // ✅ CORRIGÉ : Déduction du stock sans faire planter l'API si le stock est faible
                foreach (var produitId in listProduitId)
                {
                    int quantiteRestanteAPrelever = itemsToProcess[produitId];
                    var lotsPourCeProduit = stockLots.Where(sl => sl.ProduitId == produitId).ToList();

                    foreach (var lot in lotsPourCeProduit)
                    {
                        if (quantiteRestanteAPrelever <= 0) break;

                        int quantitePrelevee = Math.Min(lot.QuantiteRestante, quantiteRestanteAPrelever);
                        lot.QuantiteRestante -= quantitePrelevee;
                        quantiteRestanteAPrelever -= quantitePrelevee;

                        listTransation.Add(new Transaction
                        {
                            DateMouvement = DateTime.UtcNow,
                            OrderDetail = orderItems.First(od => od.ProduitId == produitId),
                            Quantite = quantitePrelevee,
                            StockLot = lot,
                            Type = TypeMouvement.Sortie
                        });
                    }

                    // Si après avoir vidé tous les lots, il manque du stock
                    if (quantiteRestanteAPrelever < 0)
                        throw new ApplicationException($"Stock insuffisant pour le produit ID {produitId}");
                }

                _context.StockLots.UpdateRange(stockLots);
                _context.Transactions.AddRange(listTransation);

                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

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