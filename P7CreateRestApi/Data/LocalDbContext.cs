using Microsoft.EntityFrameworkCore;
using Dot.Net.WebApi.Domain;
using Dot.Net.WebApi.Controllers.Domain;
using Dot.Net.WebApi.Controllers;
using System.Security.Cryptography;

namespace Dot.Net.WebApi.Data
{
    public class LocalDbContext : DbContext
    {
        public LocalDbContext(DbContextOptions<LocalDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        public DbSet<BidList> BidLists { get; set; }
        public DbSet<Trade> Trades { get; set; }
        public DbSet<CurvePoint> CurvePoints { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<RuleName> RuleNames { get; set; }
        public DbSet<User> Users { get; set; }
    }
}