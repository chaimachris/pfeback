using DeliverWholesale.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DeliverWholesale.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Suppress the PendingModelChangesWarning at the context level to allow runtime
            // detection of pending migrations without throwing an exception. Migrations
            // should be generated and applied explicitly during deployment.
            optionsBuilder.ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
            base.OnConfiguring(optionsBuilder);
        }

        public DbSet<Reclamation> Reclamations { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<AchatLot> AchatLots { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<StockLot> StockLots { get; set; }
        public DbSet<LotCommande> LotCommandes { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Categorie> Categories { get; set; }
        public DbSet<Produit> Produits { get; set; }
        public DbSet<PrixVente> PrixVentes { get; set; }
        public DbSet<Config> Configs { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Delivery> Deliveries { get; set; }
        public DbSet<Panier> Paniers { get; set; }
        public DbSet<PanierItem> PanierItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ─── Produit ────────────────────────────────────────────────
            modelBuilder.Entity<Produit>()
                .HasKey(p => p.idP);

            modelBuilder.Entity<Produit>()
                .Property(p => p.idP)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Produit>()
                .HasOne(p => p.Categorie)
                .WithMany(c => c.Produits)
                .HasForeignKey(p => p.idCategorie)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Produit>()
                .HasIndex(p => p.libelle);

            // ─── PrixVente ─────────────────────────────────────────────
            modelBuilder.Entity<PrixVente>()
                .HasKey(pv => pv.Id);

            modelBuilder.Entity<PrixVente>()
                .HasOne(pv => pv.Produit)
                .WithMany(p => p.PrixVentes)
                .HasForeignKey(pv => pv.idP)
                .OnDelete(DeleteBehavior.Cascade);

            // ─── Order ──────────────────────────────────────────────────
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>()
                .HasIndex(o => o.DateCommande);

            modelBuilder.Entity<Order>()
                .HasIndex(o => o.Statut);

            // ─── OrderDetail ────────────────────────────────────────────
            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Order)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(od => od.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Produit)
                .WithMany()
                .HasForeignKey(od => od.ProduitId)
                .OnDelete(DeleteBehavior.Restrict);

            // ─── Delivery ───────────────────────────────────────────────
            modelBuilder.Entity<Delivery>()
                .HasOne(d => d.Order)
                .WithOne(o => o.Delivery)
                .HasForeignKey<Delivery>(d => d.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // ─── Panier ────────────────────────────────────────────────
            modelBuilder.Entity<Panier>()
                .HasOne(p => p.User)
                .WithOne()
                .HasForeignKey<Panier>(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Panier>()
                .HasIndex(p => p.UserId)
                .IsUnique();

            modelBuilder.Entity<PanierItem>()
                .HasOne(i => i.Panier)
                .WithMany(p => p.Items)
                .HasForeignKey(i => i.PanierId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PanierItem>()
                .HasOne(i => i.Produit)
                .WithMany()
                .HasForeignKey(i => i.ProduitId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PanierItem>()
                .HasIndex(i => new { i.PanierId, i.ProduitId })
                .IsUnique();

            // ─── AchatLot ───────────────────────────────────────────────
            modelBuilder.Entity<AchatLot>()
                .HasOne(a => a.Produit)
                .WithMany()
                .HasForeignKey(a => a.ProduitId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AchatLot>()
                .HasOne(a => a.Supplier)
                .WithMany(s => s.AchatLots)
                .HasForeignKey(a => a.SupplierId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<AchatLot>()
                .HasIndex(a => a.NumeroLot)
                .IsUnique();

            // ─── StockLot ───────────────────────────────────────────────
            modelBuilder.Entity<StockLot>()
                .HasOne(s => s.AchatLot)
                .WithMany(a => a.StockLots)
                .HasForeignKey(s => s.AchatLotId)
                .OnDelete(DeleteBehavior.Cascade);

            // Ensure QuantiteRestante never goes negative at the DB level
            modelBuilder.Entity<StockLot>()
                .HasCheckConstraint("CK_StockLot_QuantiteRestante_NonNegative", "[QuantiteRestante] >= 0");

            // Helpful indexes for allocation queries (fast lookups of available lots per product, FIFO)
            modelBuilder.Entity<StockLot>()
                .HasIndex(s => new { s.ProduitId, s.QuantiteRestante, s.ExpirationDate, s.DateReception });

            // Configure RowVersion as concurrency token
            modelBuilder.Entity<StockLot>()
                .Property<byte[]>(s => s.RowVersion)
                .IsRowVersion();

            // ─── LotCommande ────────────────────────────────────────────
            modelBuilder.Entity<LotCommande>()
                .HasOne(l => l.StockLot)
                .WithMany(s => s.LotCommandes)
                .HasForeignKey(l => l.StockLotId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LotCommande>()
                .HasOne(l => l.OrderDetail)
                .WithMany()
                .HasForeignKey(l => l.OrderDetailId)
                .OnDelete(DeleteBehavior.Cascade);

            // ─── Transaction ────────────────────────────────────────────
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.StockLot)
                .WithMany(s => s.Transactions)
                .HasForeignKey(t => t.StockLotId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes to support audit queries: transactions by stocklot and by date
            modelBuilder.Entity<Transaction>()
                .HasIndex(t => new { t.StockLotId, t.DateMouvement });

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.OrderDetail)
                .WithMany()
                .HasForeignKey(t => t.OrderDetailId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            // ─── Reclamation ────────────────────────────────────────────
            modelBuilder.Entity<Reclamation>()
                .HasOne(r => r.ResolvedByUser)
                .WithMany()
                .HasForeignKey(r => r.ResolvedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // ─── User ───────────────────────────────────────────────────
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // ─── Config ─────────────────────────────────────────────────
            modelBuilder.Entity<Config>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<Config>()
                .Property(c => c.Id)
                .ValueGeneratedNever();

            // Explicit decimal precision to avoid silent truncation on SQL Server
            modelBuilder.Entity<OrderDetail>()
                .Property(od => od.PrixUnitaire)
                .HasPrecision(18, 2);

            modelBuilder.Entity<AchatLot>()
                .Property(a => a.PrixUnitaire)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Config>()
                .Property(c => c.FraisLivraison)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Config>()
                .Property(c => c.MontantMinimumCommande)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Config>()
                .Property(c => c.ProfitPercentage)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Order>()
                .Property(o => o.FraisLivraison)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Order>()
                .Property(o => o.TotalProduits)
                .HasPrecision(18, 2);
        }
    }
}