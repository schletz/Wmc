using Bogus;
using Microsoft.EntityFrameworkCore;
using Spengernews.Application.Model;
using System;
using System.Linq;

namespace Spengernews.Application.Infrastructure
{
    public class SpengernewsContext : DbContext
    {
        public DbSet<Article> Articles => Set<Article>();
        public DbSet<Author> Authors => Set<Author>();
        public DbSet<Category> Categories => Set<Category>();

        public SpengernewsContext(DbContextOptions<SpengernewsContext> opt) : base(opt) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                // Generic config for all entities
                // ON DELETE RESTRICT instead of ON DELETE CASCADE
                foreach (var key in entityType.GetForeignKeys())
                    key.DeleteBehavior = DeleteBehavior.Restrict;

                foreach (var prop in entityType.GetDeclaredProperties())
                {
                    // Define Guid as alternate key. The database can create a guid fou you.
                    if (prop.Name == "Guid")
                    {
                        modelBuilder.Entity(entityType.ClrType).HasAlternateKey("Guid");
                        prop.ValueGenerated = Microsoft.EntityFrameworkCore.Metadata.ValueGenerated.OnAdd;
                    }
                    // Default MaxLength of string Properties is 255.
                    if (prop.ClrType == typeof(string) && prop.GetMaxLength() is null) prop.SetMaxLength(255);
                    // Seconds with 3 fractional digits.
                    if (prop.ClrType == typeof(DateTime)) prop.SetPrecision(3);
                    if (prop.ClrType == typeof(DateTime?)) prop.SetPrecision(3);
                }
            }
        }

        public void Seed()
        {
            Randomizer.Seed = new Random(1039);
            var faker = new Faker("de");

            var authors = new Faker<Author>("de").CustomInstantiator(f =>
            {
                var lastname = f.Name.LastName();
                return new Author(
                    firstname: f.Name.FirstName(),
                    lastname: lastname,
                    email: $"{lastname.ToLower()}@spengergasse.at",
                    username: lastname.ToLower(),
                    initialPassword: "1111",
                    phone: $"{+43}{f.Random.Int(1, 9)}{f.Random.String2(9, "0123456789")}".OrNull(f, 0.25f))
                { Guid = f.Random.Guid() };
            })
            .Generate(10)
            .GroupBy(a => a.Email).Select(g => g.First())
            .ToList();
            Authors.AddRange(authors);
            SaveChanges();

            var categories = new Faker<Category>("de").CustomInstantiator(f =>
            {
                return new Category(f.Commerce.ProductAdjective())
                { Guid = f.Random.Guid() };
            })
            .Generate(10)
            .GroupBy(c => c.Name).Select(g => g.First())
            .ToList();
            Categories.AddRange(categories);
            SaveChanges();

            var articles = new Faker<Article>("de").CustomInstantiator(f =>
            {
                return new Article(
                    headline: f.Lorem.Sentence(f.Random.Int(2, 4)),
                    content: f.Lorem.Paragraphs(10, 20),
                    created: f.Date.Between(new DateTime(2021, 1, 1), new DateTime(2022, 1, 1)),
                    imageUrl: f.Image.PicsumUrl(),
                    author: f.Random.ListItem(authors),
                    category: f.Random.ListItem(categories))
                { Guid = f.Random.Guid() };
            })
            .Generate(6)
            .ToList();
            Articles.AddRange(articles);
            SaveChanges();
        }
    }
}
