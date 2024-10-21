using Microsoft.EntityFrameworkCore;
using P7CreateRestApi.Domain;
using System.Reflection.Emit;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using P7CreateRestApi.Controllers.Domain;
using P7CreateRestApi.Controllers;

namespace P7CreateRestApi.Data
{
    public class LocalDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public LocalDbContext(DbContextOptions<LocalDbContext> options) : base(options) { }

        public DbSet<BidList> BidLists { get; set; }
        public DbSet<CurvePoint> CurvePoints { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<RuleName> RuleNames { get; set; }
        public DbSet<Trade> Trades { get; set; }
        public DbSet<User> Users { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<BidList>()
           .HasKey(cp => cp.BidListId);
            builder.Entity<CurvePoint>()
           .HasKey(cp => cp.Id);
            builder.Entity<Rating>()
           .HasKey(cp => cp.Id);
            builder.Entity<RuleName>()
          .HasKey(cp => cp.Id);
            builder.Entity<Trade>()
         .HasKey(cp => cp.TradeId);
            builder.Entity<User>()
         .HasKey(cp => cp.Id);

            // Additional configuration for Identity
            builder.Entity<IdentityUserLogin<int>>().HasKey(l => new { l.LoginProvider, l.ProviderKey });
            builder.Entity<IdentityUserRole<int>>().HasKey(r => new { r.UserId, r.RoleId });
            builder.Entity<IdentityUserToken<int>>().HasKey(t => new { t.UserId, t.LoginProvider, t.Name });

            // Seed data
            builder.Entity<BidList>().HasData(
                new BidList
                {
                    BidListId = 1,
                    Account = "user",
                    BidType = "firstType",
                    BidQuantity = 2,
                    AskQuantity = 3,
                    Bid = 0,
                    Ask = 0,
                    Benchmark = "string",
                    BidListDate = new DateTime(2024, 10, 07),
                    Commentary = "string",
                    BidSecurity = "string",
                    BidStatus = "string",
                    Trader = "string",
                    Book = "string",
                    CreationName = "string",
                    CreationDate = new DateTime(2024, 10, 07),
                    RevisionName = "string",
                    RevisionDate = new DateTime(2024, 10, 07),
                    DealName = "string",
                    DealType = "string",
                    SourceListId = "string",
                    Side = "string"
                });
        }
    }

}