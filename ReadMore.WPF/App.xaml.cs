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

            // DbContext opties instellen
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=ReadMoreDb;Trusted_Connection=True;")
                .Options;

            // Database migreren & admin seeden
            using (var context = new ApplicationDbContext(options))
            {
                context.Database.Migrate();       // Zorgt dat database up-to-date is
                context.SeedAdminUser();          // Dynamisch admin toevoegen
            }

            // Login window tonen
            new LoginWindow().Show();
        }
    }
}
