using System.Linq;
using System.Windows;
using ReadMore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace ReadMore.WPF
{
    public partial class LoginWindow : Window
    {
        private readonly ApplicationDbContext _context;

        public LoginWindow()
        {
            InitializeComponent();

            // DbContext instellen
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=ReadMoreDb;Trusted_Connection=True;")
                .Options;

            _context = new ApplicationDbContext(options);
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            var user = _context.Users.FirstOrDefault(u => u.UserName == username);

            if (user != null && !string.IsNullOrEmpty(user.PasswordHash))
            {
                var hasher = new PasswordHasher<ApplicationUser>();
                var result = hasher.VerifyHashedPassword(user, user.PasswordHash, password);

                if (result == PasswordVerificationResult.Success)
                {
                    // Login succesvol: open MainWindow
                    var mainWindow = new MainWindow(_context, user);
                    mainWindow.Show();
                    this.Close();
                    return;
                }
            }

            MessageBox.Show("Onjuiste gebruikersnaam of wachtwoord.", "Login mislukt", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
