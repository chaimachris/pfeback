using DeliverWholesale.Data;
using DeliverWholesale.DTOs;
using DeliverWholesale.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DeliverWholesale.Handler.Orders
{
    public class CreateOrderCommand : IRequest<int>
    {
        public OrderCreateDto OrderDto { get; set; }

        public CreateOrderCommand(OrderCreateDto dto)
        {
            OrderDto = dto;
        }
    }

    public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, int>
    {
        private readonly ApplicationDbContext _context;

        public CreateOrderHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            if (request.OrderDto.Items == null || !request.OrderDto.Items.Any())
                throw new Exception("Aucun produit dans la commande.");

            foreach (var item in request.OrderDto.Items)
            {
                if (item.Quantite <= 0)
                    throw new Exception($"Quantité invalide pour le produit {item.ProduitId}.");

                var produitExiste = await _context.Produits.AnyAsync(p => p.Id == item.ProduitId);
                if (!produitExiste)
                    throw new Exception($"Produit avec ID {item.ProduitId} introuvable.");
            }

            var order = new Order
            {
                UserId = 1, 
                DateCommande = DateTime.UtcNow,
                FraisLivraison = 10, 
                TotalProduits = 0, 
                OrderDetails = request.OrderDto.Items.Select(i => new OrderDetail
                {
                    ProduitId = i.ProduitId,
                    Quantite = i.Quantite
                }).ToList()
            };

            order.TotalProduits = order.OrderDetails.Sum(d => d.Quantite * 100);

            _context.Orders.Add(order);

            try
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex)
            {
                throw new Exception(ex.InnerException?.Message ?? ex.Message);
            }

            return order.Id;
        }
    }
}