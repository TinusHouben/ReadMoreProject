using System.Windows;
using Microsoft.EntityFrameworkCore;
using ReadMore.Models;

namespace ReadMore.WPF
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=ReadMoreDb;Trusted_Connection=True;")
                .Options;

            using (var context = new ApplicationDbContext(options))
            {
                context.Database.Migrate();
                context.SeedAdminUser();
            }

            new LoginWindow().Show();
        }
    }
}
