using Final_NET.Models;
using Microsoft.EntityFrameworkCore;

namespace Final_NET
{
    public class TravelNewsDBContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Articles> Articles { get; set; }
        public TravelNewsDBContext(DbContextOptions<TravelNewsDBContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>()
               .Property(a => a.Id)
               .ValueGeneratedOnAdd();

            modelBuilder.Entity<Account>()
                .Property(a => a.Role)
                .HasDefaultValue("Viewer");

            modelBuilder.Entity<Account>()
                .Property(a => a.Status)
                .HasDefaultValue(1);

            modelBuilder.Entity<Articles>()
                .Property(a => a.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Articles>()
                .Property(a => a.View)
                .HasDefaultValue(0);

            modelBuilder.Entity<Articles>()
                .Property(a => a.Date)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Articles>()
                .HasOne(a => a.Account)
                .WithMany(a => a.Articles)
                .HasForeignKey(a => a.AccountId);
        }
    }
}
