using Authenticator.Models;
using Microsoft.EntityFrameworkCore;

namespace Authenticator.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<Stocks> StockTable { get; set; }
        public DbSet<Accounts> AccountsTable { get; set; }
        public DbSet<Portfolio> PortfolioTable { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Portfolio>(x => x.HasKey(p => new { p.AccountId, p.StockId }));
            builder.Entity<Portfolio>()
                .HasOne(u => u.account)
                .WithMany(u => u.Portfolios)
                .HasForeignKey(u => u.AccountId);

            builder.Entity<Portfolio>()
               .HasOne(u => u.stocks)
               .WithMany(u => u.Portfolios)
               .HasForeignKey(u => u.StockId);
        }
    }
}