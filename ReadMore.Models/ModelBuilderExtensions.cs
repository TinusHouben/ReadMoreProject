using Microsoft.EntityFrameworkCore;

namespace ReadMore.Models
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Comic>().HasData(
                new Comic { Id = 1, Title = "Spider-Man", Author = "Stan Lee", Series = "Marvel", NumberInSeries = 1, Price = 12.99M, IsDeleted = false },
                new Comic { Id = 2, Title = "Batman", Author = "Bob Kane", Series = "DC", NumberInSeries = 1, Price = 15.99M, IsDeleted = false }
            );

            modelBuilder.Entity<Book>().HasData(
                new Book { Id = 1, Title = "C# in Depth", Author = "Jon Skeet", Price = 45.00M, IsDeleted = false }
            );
        }
    }
}
