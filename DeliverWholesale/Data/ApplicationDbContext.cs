using DeliverWholesale.Models;
using Microsoft.EntityFrameworkCore;

namespace DeliverWholesale.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Categorie> Categories { get; set; }
        public DbSet<Produit> Produits { get; set; }
        public DbSet<Config> Configs { get; set; } 
        public DbSet<StockLot> StockLots { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Delivery> Deliveries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

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

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.StockLot)
                .WithMany(sl => sl.Transactions)
                .HasForeignKey(t => t.StockLotId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.OrderDetail)
                .WithMany()
                .HasForeignKey(t => t.OrderDetailId)
                .IsRequired(false) 
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Delivery>()
                .HasOne(d => d.Order)
                .WithOne(o => o.Delivery)
                .HasForeignKey<Delivery>(d => d.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Produit>()
                .HasOne(p => p.Categorie)
                .WithMany(c => c.Produits)
                .HasForeignKey(p => p.CategorieId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<StockLot>()
                .HasOne(s => s.Produit)
                .WithMany(p => p.StockLots)
                .HasForeignKey(s => s.ProduitId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Produit>()
                .HasIndex(p => p.Nom);

            modelBuilder.Entity<Order>()
                .HasIndex(o => o.DateCommande);

            modelBuilder.Entity<Order>()
                .HasIndex(o => o.Statut);

            modelBuilder.Entity<Config>()
                .HasKey(c => c.Id);
            modelBuilder.Entity<Config>()
                .Property(c => c.Id)
                .ValueGeneratedNever();
        }
    }
}