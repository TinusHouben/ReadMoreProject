using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace ReadMore.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Comic> Comics { get; set; }
        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Book>().Property(b => b.Price).HasColumnType("decimal(18,2)");
            builder.Entity<Comic>().Property(c => c.Price).HasColumnType("decimal(18,2)");
            builder.Entity<Order>().Property(o => o.TotalPrice).HasColumnType("decimal(18,2)");

            builder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = "1", Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole { Id = "2", Name = "User", NormalizedName = "USER" }
            );

            builder.Entity<Book>().HasQueryFilter(b => !b.IsDeleted);
            builder.Entity<Comic>().HasQueryFilter(c => !c.IsDeleted);
            builder.Entity<Order>().HasQueryFilter(o => !o.IsDeleted);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ConfigureWarnings(warnings =>
                warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
        }
    }

    public static class DbInitializer
    {
        public static void SeedAdminUser(this ApplicationDbContext context)
        {
            if (!context.Users.Any(u => u.NormalizedUserName == "ADMIN"))
            {
                var admin = new ApplicationUser
                {
                    UserName = "admin",
                    NormalizedUserName = "ADMIN",
                    Email = "admin@readmore.com",
                    NormalizedEmail = "ADMIN@READMORE.COM",
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString()
                };

                var hasher = new PasswordHasher<ApplicationUser>();
                admin.PasswordHash = hasher.HashPassword(admin, "Admin123!");

                context.Users.Add(admin);

                var adminRole = context.Roles.FirstOrDefault(r => r.NormalizedName == "ADMIN");
                if (adminRole == null)
                {
                    adminRole = new IdentityRole { Id = "1", Name = "Admin", NormalizedName = "ADMIN" };
                    context.Roles.Add(adminRole);
                    context.SaveChanges();
                }

                context.UserRoles.Add(new IdentityUserRole<string>
                {
                    UserId = admin.Id,
                    RoleId = adminRole.Id
                });

                context.SaveChanges();
            }
        }
    }
}
