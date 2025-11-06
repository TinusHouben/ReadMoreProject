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
                    var mainWindow = new MainWindow(_context, user);
                    mainWindow.Show();
                    this.Close();
                    return;
                }
            }

            MessageBox.Show(
                "Onjuiste gebruikersnaam of wachtwoord.",
                "Login mislukt",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show(
                    "Vul een gebruikersnaam en wachtwoord in.",
                    "Registratie mislukt",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }

            var existingUser = _context.Users.FirstOrDefault(u => u.UserName == username);
            if (existingUser != null)
            {
                MessageBox.Show(
                    "Deze gebruikersnaam is al in gebruik.",
                    "Registratie mislukt",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }

            var newUser = new ApplicationUser
            {
                UserName = username,
                NormalizedUserName = username.ToUpper(),
                Email = $"{username}@readmore.com",
                NormalizedEmail = $"{username}@readmore.com".ToUpper(),
                EmailConfirmed = true
            };

            var hasher = new PasswordHasher<ApplicationUser>();
            newUser.PasswordHash = hasher.HashPassword(newUser, password);

            _context.Users.Add(newUser);
            _context.SaveChanges();

            MessageBox.Show(
                "Registratie succesvol! Je kunt nu inloggen.",
                "Succes",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }
    }
}
