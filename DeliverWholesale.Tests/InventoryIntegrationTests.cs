using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;
using DeliverWholesale.Infrastructure.Services;
using DeliverWholesale.Tests.TestHelpers;
using DeliverWholesale.Domain.Entities;
using DeliverWholesale.Domain.Enums;

namespace DeliverWholesale.Tests
{
    public class InventoryIntegrationTests
    {
        [Fact]
        public async Task PurchaseCreatesAchatStockTransactionAtomically()
        {
            using var conn = TestDbFactory.CreateOpenConnection();
            using var ctx = TestDbFactory.CreateContext(conn);

            // seed product
            var produit = new Produit { libelle = "P1", NbUnite = 1 };
            ctx.Produits.Add(produit);
            await ctx.SaveChangesAsync();

            var svc = new InventoryService(ctx);

            var achatId = await svc.ReceivePurchaseStockAsync(produit.idP, 10, 5m);

            Assert.NotEqual(0, achatId);
            var achat = await ctx.AchatLots.FindAsync(achatId);
            Assert.NotNull(achat);

            var stock = await ctx.StockLots.FirstOrDefaultAsync(s => s.AchatLotId == achatId);
            Assert.NotNull(stock);

            var tx = await ctx.Transactions.FirstOrDefaultAsync(t => t.StockLotId == stock.Id && t.Type == TypeMouvement.Entree);
            Assert.NotNull(tx);
            Assert.Equal(stock.QuantiteRestante, tx.Quantite);
        }

        [Fact]
        public async Task AllocationConsumesMultipleLotsAndCreatesLotCommandes()
        {
            using var conn = TestDbFactory.CreateOpenConnection();
            using var ctx = TestDbFactory.CreateContext(conn);

            var produit = new Produit { libelle = "P2", NbUnite = 1 };
            ctx.Produits.Add(produit);
            await ctx.SaveChangesAsync();

            // create two stock lots
            var a1 = new AchatLot { ProduitId = produit.idP, QuantiteAchetee = 5, PrixUnitaire = 1, Fournisseur = "X", NumeroLot = "A1" };
            var s1 = new StockLot { AchatLot = a1, QuantiteRestante = 5, ProduitId = produit.idP, DateReception = DateTime.UtcNow.AddDays(-2) };

            var a2 = new AchatLot { ProduitId = produit.idP, QuantiteAchetee = 7, PrixUnitaire = 1, Fournisseur = "X", NumeroLot = "A2" };
            var s2 = new StockLot { AchatLot = a2, QuantiteRestante = 7, ProduitId = produit.idP, DateReception = DateTime.UtcNow.AddDays(-1) };

            ctx.StockLots.AddRange(s1, s2);
            await ctx.SaveChangesAsync();

            var svc = new InventoryService(ctx);

            var items = new[] { new AllocateItem(produit.idP, 99, 10) };
            var allocation = await svc.AllocateOrderStockAsync(1, items);

            // two lot commandes should be created for product
            var commandes = ctx.LotCommandes.Where(l => l.OrderDetailId == 99).ToList();
            Assert.Equal(2, commandes.Count);

            // quantities consumed correctly
            var s1After = await ctx.StockLots.FindAsync(s1.Id);
            var s2After = await ctx.StockLots.FindAsync(s2.Id);
            Assert.Equal(0, s1After.QuantiteRestante);
            Assert.Equal(2, s2After.QuantiteRestante);
        }

        [Fact]
        public async Task AllocationFailsWhenInsufficientStock_NoNegative()
        {
            using var conn = TestDbFactory.CreateOpenConnection();
            using var ctx = TestDbFactory.CreateContext(conn);

            var produit = new Produit { libelle = "P3", NbUnite = 1 };
            ctx.Produits.Add(produit);
            await ctx.SaveChangesAsync();

            var a1 = new AchatLot { ProduitId = produit.idP, QuantiteAchetee = 2, PrixUnitaire = 1, Fournisseur = "X", NumeroLot = "B1" };
            var s1 = new StockLot { AchatLot = a1, QuantiteRestante = 2, ProduitId = produit.idP };
            ctx.StockLots.Add(s1);
            await ctx.SaveChangesAsync();

            var svc = new InventoryService(ctx);

            var items = new[] { new AllocateItem(produit.idP, 1, 5) };
            await Assert.ThrowsAsync<ApplicationException>(() => svc.AllocateOrderStockAsync(1, items));

            // ensure no negative stock
            var sAfter = await ctx.StockLots.FindAsync(s1.Id);
            Assert.True(sAfter.QuantiteRestante >= 0);
        }

        [Fact]
        public async Task RemoveStockHandler_WritesLedgerEntry()
        {
            using var conn = TestDbFactory.CreateOpenConnection();
            using var ctx = TestDbFactory.CreateContext(conn);

            var produit = new Produit { libelle = "P4", NbUnite = 1 };
            ctx.Produits.Add(produit);
            await ctx.SaveChangesAsync();

            var a = new AchatLot { ProduitId = produit.idP, QuantiteAchetee = 10, PrixUnitaire = 1, Fournisseur = "X", NumeroLot = "C1" };
            var s = new StockLot { AchatLot = a, QuantiteRestante = 10, ProduitId = produit.idP };
            ctx.StockLots.Add(s);
            await ctx.SaveChangesAsync();

            var svc = new InventoryService(ctx);
            var handler = new Application.Features.Handler.Stock.RemoveStockHandler(ctx, svc);

            var cmd = new Application.Features.Handler.Stock.RemoveStockCommand(s.Id, 3);
            var res = await handler.Handle(cmd, default);
            Assert.True(res);

            var tx = await ctx.Transactions.FirstOrDefaultAsync(t => t.StockLotId == s.Id && t.Type == TypeMouvement.Sortie && t.Quantite == 3);
            Assert.NotNull(tx);
            var sAfter = await ctx.StockLots.FindAsync(s.Id);
            Assert.Equal(7, sAfter.QuantiteRestante);
        }

        [Fact]
        public async Task RevertOrderRestoresStockAndCreatesAdjustments()
        {
            using var conn = TestDbFactory.CreateOpenConnection();
            using var ctx = TestDbFactory.CreateContext(conn);

            var produit = new Produit { libelle = "P5", NbUnite = 1 };
            ctx.Produits.Add(produit);
            await ctx.SaveChangesAsync();

            // create lots
            var s1 = new StockLot { AchatLot = new AchatLot { ProduitId = produit.idP, QuantiteAchetee = 5, PrixUnitaire = 1, Fournisseur = "X", NumeroLot = "D1" }, QuantiteRestante = 5, ProduitId = produit.idP };
            var s2 = new StockLot { AchatLot = new AchatLot { ProduitId = produit.idP, QuantiteAchetee = 5, PrixUnitaire = 1, Fournisseur = "X", NumeroLot = "D2" }, QuantiteRestante = 5, ProduitId = produit.idP };
            ctx.StockLots.AddRange(s1, s2);
            await ctx.SaveChangesAsync();

            var svc = new InventoryService(ctx);

            // allocate 6 units for order with details
            var items = new[] { new AllocateItem(produit.idP, 201, 6) };
            await svc.AllocateOrderStockAsync(10, items);

            // simulate order exists with OrderDetails id 201
            var order = new Order { DateCommande = DateTime.UtcNow };
            order.OrderDetails.Add(new OrderDetail { Id = 201, Order = order, ProduitId = produit.idP, Quantite = 6, PrixUnitaire = 1 });
            ctx.Orders.Add(order);
            await ctx.SaveChangesAsync();

            // revert
            await svc.RevertOrderStockTransactionalAsync(order.Id);
            await ctx.SaveChangesAsync();

            // after revert, stock quantities should be restored to original total (10)
            var totals = await ctx.StockLots.Where(sl => sl.ProduitId == produit.idP).SumAsync(sl => sl.QuantiteRestante);
            Assert.Equal(10, totals);

            // adjustments should exist
            var adjustments = await ctx.Transactions.Where(t => t.Type == TypeMouvement.Ajustement && t.OrderDetailId == null).ToListAsync();
            Assert.True(adjustments.Count > 0);
        }

        [Fact]
        public async Task ConcurrentAllocations_DoNotOversell()
        {
            // shared in-memory connection to allow concurrent DbContexts
            var connection = TestDbFactory.CreateOpenConnection();
            try
            {
                using var ctxInit = TestDbFactory.CreateContext(connection);
                var produit = new Produit { libelle = "P6", NbUnite = 1 };
                ctxInit.Produits.Add(produit);
                await ctxInit.SaveChangesAsync();

                var s = new StockLot { AchatLot = new AchatLot { ProduitId = produit.idP, QuantiteAchetee = 10, PrixUnitaire = 1, Fournisseur = "X", NumeroLot = "E1" }, QuantiteRestante = 10, ProduitId = produit.idP };
                ctxInit.StockLots.Add(s);
                await ctxInit.SaveChangesAsync();
                await ctxInit.DisposeAsync();

                // run two concurrent allocations using separate contexts
                var task1 = Task.Run(async () =>
                {
                    using var c1 = TestDbFactory.CreateContext(connection);
                    var svc1 = new InventoryService(c1);
                    try
                    {
                        var res = await svc1.AllocateOrderStockAsync(1, new[] { new AllocateItem(produit.idP, 301, 7) });
                        return res.Values.SelectMany(x => x).Sum(p => p.QuantitePrelevee);
                    }
                    catch
                    {
                        return 0;
                    }
                });

                var task2 = Task.Run(async () =>
                {
                    using var c2 = TestDbFactory.CreateContext(connection);
                    var svc2 = new InventoryService(c2);
                    try
                    {
                        var res = await svc2.AllocateOrderStockAsync(2, new[] { new AllocateItem(produit.idP, 302, 7) });
                        return res.Values.SelectMany(x => x).Sum(p => p.QuantitePrelevee);
                    }
                    catch
                    {
                        return 0;
                    }
                });

                await Task.WhenAll(task1, task2);

                var totalAllocated = task1.Result + task2.Result;

                using var ctxCheck = TestDbFactory.CreateContext(connection);
                var remaining = await ctxCheck.StockLots.Where(sl => sl.ProduitId == produit.idP).SumAsync(sl => sl.QuantiteRestante);
                Assert.True(totalAllocated + remaining <= 10);
            }
            finally
            {
                connection.Dispose();
            }
        }

        [Fact]
        public async Task ExpiredLots_AreExcludedFromAllocation()
        {
            using var conn = TestDbFactory.CreateOpenConnection();
            using var ctx = TestDbFactory.CreateContext(conn);

            var produit = new Produit { libelle = "P7", NbUnite = 1 };
            ctx.Produits.Add(produit);
            await ctx.SaveChangesAsync();

            // expired lot
            var expired = new StockLot { AchatLot = new AchatLot { ProduitId = produit.idP, QuantiteAchetee = 5, PrixUnitaire = 1, Fournisseur = "X", NumeroLot = "F1" }, QuantiteRestante = 5, ProduitId = produit.idP, ExpirationDate = DateTime.UtcNow.AddDays(-1) };
            // valid lot
            var valid = new StockLot { AchatLot = new AchatLot { ProduitId = produit.idP, QuantiteAchetee = 5, PrixUnitaire = 1, Fournisseur = "X", NumeroLot = "F2" }, QuantiteRestante = 5, ProduitId = produit.idP, ExpirationDate = DateTime.UtcNow.AddDays(10) };
            ctx.StockLots.AddRange(expired, valid);
            await ctx.SaveChangesAsync();

            var svc = new InventoryService(ctx);

            var allocation = await svc.AllocateOrderStockAsync(1, new[] { new AllocateItem(produit.idP, 401, 4) });

            // ensure expired lot not used
            var usedFromExpired = allocation.Values.SelectMany(l => l).Any(p => p.StockLotId == expired.Id);
            Assert.False(usedFromExpired);

            var validAfter = await ctx.StockLots.FindAsync(valid.Id);
            Assert.Equal(1, validAfter.QuantiteRestante);
        }
    }
}
