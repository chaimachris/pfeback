using DeliverWholesale.Domain.Entities;
using DeliverWholesale.Domain.Enums;
using DeliverWholesale.Domain.Entities;
using DeliverWholesale.Infrastructure.Data;
using DeliverWholesale.Infrastructure.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DeliverWholesale.Application.Features.Handler.Orders
{
    public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, int>
    {
        private readonly ApplicationDbContext _context;
        private readonly NotificationService _notification;

        public CreateOrderHandler(
            ApplicationDbContext context,
            NotificationService notification)
        {
            _context = context;
            _notification = notification;
        }

        public async Task<int> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            if (request.OrderDto.Items == null || !request.OrderDto.Items.Any())
                throw new ApplicationException("Aucun produit dans la commande.");

            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                var user = await _context.Users.FirstAsync(cancellationToken);

                var order = new Order
                {
                    UserId = user.Id,
                    DateCommande = DateTime.UtcNow,
                    FraisLivraison = 10,
                    TotalProduits = 0,
                    OrderDetails = new List<OrderDetail>()
                };

                _context.Orders.Add(order);

                decimal total = 0;

                foreach (var item in request.OrderDto.Items)
                {
                    if (item.Quantite <= 0)
                        throw new ApplicationException($"Quantité invalide pour produit {item.ProduitId}");

                    var produit = await _context.Produits
                        .FirstOrDefaultAsync(p => p.Id == item.ProduitId, cancellationToken);

                    if (produit == null)
                        throw new ApplicationException($"Produit {item.ProduitId} introuvable");

                    var detail = new OrderDetail
                    {
                        Order = order,
                        ProduitId = item.ProduitId,
                        Quantite = item.Quantite,
                        PrixUnitaire = produit.PrixAchat
                    };

                    _context.OrderDetails.Add(detail);

                    total += detail.Quantite * detail.PrixUnitaire;

                    //  FIFO STOCK
                    int reste = item.Quantite;

                    var stockLots = await _context.StockLots
                        .Include(s => s.AchatLot)
                        .Where(s => s.AchatLot.ProduitId == item.ProduitId && s.QuantiteRestante > 0)
                        .OrderBy(s => s.DateReception)
                        .ToListAsync(cancellationToken);

                    foreach (var lot in stockLots)
                    {
                        if (reste == 0)
                            break;

                        int preleve = Math.Min(lot.QuantiteRestante, reste);

                        if (preleve <= 0) continue;

                        lot.QuantiteRestante -= preleve;

                        _context.LotCommandes.Add(new LotCommande
                        {
                            StockLot = lot,
                            OrderDetail = detail,
                            QuantitePrelevee = preleve
                        });

                        _context.Transactions.Add(new Transaction
                        {
                            StockLot = lot,
                            OrderDetail = detail,
                            Type = TypeMouvement.Sortie,
                            Quantite = preleve,
                            DateMouvement = DateTime.UtcNow
                        });

                        reste -= preleve;
                    }

                    if (reste > 0)
                        throw new ApplicationException($"Stock insuffisant pour produit {item.ProduitId}");
                }

                order.TotalProduits = total;

              
                await _context.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);

                //  NOTIFICATION ADMIN
                await _notification.NotifyAdmins(
                    $"Nouvelle commande créée (ID: {order.Id}) par {user.Prenom} {user.Nom}",
                    "CREATE_ORDER"
                );

                return order.Id;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}