using EntityFramework.Models;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Data
{
    public class MarketDbContext : DbContext
    {
        public MarketDbContext(DbContextOptions<MarketDbContext> options) : base(options)
        {
        }

        public DbSet<Client> Client { get; set; }
        public DbSet<Brokerage> Brokerage { get; set; }
        public DbSet<Subscription> Subscription { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Client>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<Client>()
                .ToTable("client");

            modelBuilder.Entity<Brokerage>()
                .HasKey(b => b.Id);

            modelBuilder.Entity<Brokerage>()
                .ToTable("brokerage");

            modelBuilder.Entity<Subscription>()
            .HasKey(t => new { t.BrokerageId, t.ClientId });

            modelBuilder.Entity<Subscription>()
                .ToTable("subscription");

            modelBuilder.Entity<Subscription>()
                .HasOne<Brokerage>(s => s.Brokerage)
                .WithMany(b => b.Subscriptions)
                .HasForeignKey(s => s.BrokerageId);

            modelBuilder.Entity<Subscription>()
                .HasOne<Client>(s => s.Client)
                .WithMany(b => b.Subscriptions)
                .HasForeignKey(s => s.ClientId);
        }
    }
}