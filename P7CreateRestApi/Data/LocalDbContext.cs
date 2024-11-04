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

            // Seed data for roles
            var adminRole = new IdentityRole<int> { Id = 1, Name = "Admin", NormalizedName = "ADMIN" };
            var userRole = new IdentityRole<int> { Id = 2, Name = "User", NormalizedName = "USER" };

            builder.Entity<IdentityRole<int>>().HasData(adminRole, userRole);

            // Password hasher for seeding users with hashed passwords
            var hasher = new PasswordHasher<User>();

            // Seed data for users
            var adminUser = new User
            {
                Id = 1,
                Fullname = "Admin Primary",
                UserName = "admin",
                Role = "Admin",
                NormalizedUserName = "ADMIN",
                Email = "admin@example.com",
                NormalizedEmail = "ADMIN@EXAMPLE.COM",
                EmailConfirmed = true,
                PasswordHash = hasher.HashPassword(null, "AdminPassword123!"),
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var normalUser = new User
            {
                Id = 2,
                Fullname = "User Primary",
                UserName = "user",
                Role = "User",
                NormalizedUserName = "USER",
                Email = "user@example.com",
                NormalizedEmail = "USER@EXAMPLE.COM",
                EmailConfirmed = true,
                PasswordHash = hasher.HashPassword(null, "UserPassword123!"),
                SecurityStamp = Guid.NewGuid().ToString()
            };

            builder.Entity<User>().HasData(adminUser, normalUser);

            // Assign roles to users
            builder.Entity<IdentityUserRole<int>>().HasData(
                new IdentityUserRole<int> { UserId = adminUser.Id, RoleId = adminRole.Id },
                new IdentityUserRole<int> { UserId = normalUser.Id, RoleId = userRole.Id }
            );


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