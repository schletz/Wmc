using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using webapi.Model;

namespace webapi.Infrastructure
{
    public class SpengernewsContext : DbContext
    {
        public DbSet<Article> Articles => Set<Article>();
        public DbSet<Author> Authors => Set<Author>();
        public DbSet<Category> Categories => Set<Category>();

        public SpengernewsContext(DbContextOptions<SpengernewsContext> opt): base(opt) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }

        public void Seed()
        {

        }
    }
}
