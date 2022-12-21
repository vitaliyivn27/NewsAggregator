using Microsoft.EntityFrameworkCore;
using NewsAggregator.DataBase.Entities;

namespace NewsAggregator.DataBase
{
    public class GoodNewsAggregatorContext : DbContext
    {
        public DbSet<Article?> Articles { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Source> Sources { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        public GoodNewsAggregatorContext(DbContextOptions<GoodNewsAggregatorContext> options)
            : base(options)
        {
        }
    }
}