using System.Linq;
using System.Windows;
using Microsoft.AspNetCore.Identity;
using ReadMore.Models;

namespace ReadMore.WPF
{
    public partial class RegisterWindow : Window
    {
        private readonly ApplicationDbContext _context;

        public RegisterWindow(ApplicationDbContext context)
        {
            InitializeComponent();
            _context = context;
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text.Trim();
            string email = EmailTextBox.Text.Trim();
            string password = PasswordBox.Password;
            string confirmPassword = ConfirmPasswordBox.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vul alle velden in.", "Fout", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Wachtwoorden komen niet overeen.", "Fout", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_context.Users.Any(u => u.UserName == username))
            {
                MessageBox.Show("Deze gebruikersnaam bestaat al.", "Fout", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var user = new ApplicationUser
            {
                UserName = username,
                NormalizedUserName = username.ToUpper(),
                Email = email,
                NormalizedEmail = email.ToUpper(),
                EmailConfirmed = true
            };

            var hasher = new PasswordHasher<ApplicationUser>();
            user.PasswordHash = hasher.HashPassword(user, password);

            _context.Users.Add(user);
            _context.SaveChanges();

            MessageBox.Show("Registratie succesvol! Je kunt nu inloggen.", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }
    }
}
