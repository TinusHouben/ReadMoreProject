using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using ReadMore.Models;
using Microsoft.AspNetCore.Identity;

namespace ReadMore.WPF
{
    public partial class MainWindow : Window
    {
        private readonly ApplicationDbContext _context;
        private readonly ApplicationUser _currentUser;

        private List<Book> _catalogBooks = new();
        private List<AdminOrderView> _adminOrders = new();
        private List<ApplicationUser> _users = new();
        private List<ContactMessage> _contactMessages = new();

        public MainWindow(ApplicationDbContext context, ApplicationUser user)
        {
            InitializeComponent();
            _context = context;
            _currentUser = user;

            ConfigureAccess();

            LoadCatalogus();

            if (!IsAdmin())
            {
                LoadOrdersPublic();
            }
            else
            {
                LoadAdminBooks();
                LoadAdminOrders();
                LoadUsers();
                LoadContactMessages();
            }
        }

        #region Access & Tabs
        private bool IsAdmin()
        {
            return _context.UserRoles
                .Any(ur => ur.UserId == _currentUser.Id && _context.Roles.Any(r => r.Id == ur.RoleId && r.Name == "Admin"));
        }

        private void ConfigureAccess()
        {
            bool isAdmin = IsAdmin();
            Title = $"📚 ReadMore - Ingelogd als {_currentUser.UserName} {(isAdmin ? "(Admin)" : "(Gebruiker)")}";

            foreach (var tab in MainTabControl.Items.OfType<System.Windows.Controls.TabItem>().ToList())
            {
                string header = tab.Header?.ToString() ?? "";

                if (isAdmin)
                {
                    // Verwijder tabs voor gewone gebruiker
                    if (header == "📚 Catalogus" || header == "📦 Bestellingen" || header == "📬 Contact")
                        MainTabControl.Items.Remove(tab);
                }
                else
                {
                    // Verwijder admin tabs
                    if (header != "📚 Catalogus" && header != "📦 Bestellingen" && header != "📬 Contact")
                        MainTabControl.Items.Remove(tab);
                }
            }
        }
        #endregion

        #region Catalogus
        private void LoadCatalogus()
        {
            _catalogBooks = _context.Books.Where(b => !b.IsDeleted).ToList();
            CatalogusDataGrid.ItemsSource = _catalogBooks;
        }

        private void CatalogusSearchTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            string q = CatalogusSearchTextBox.Text?.Trim() ?? string.Empty;
            CatalogusPlaceholder.Visibility = string.IsNullOrEmpty(q) ? Visibility.Visible : Visibility.Hidden;

            CatalogusDataGrid.ItemsSource = _catalogBooks
                .Where(b => b.Title.Contains(q, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        private void CreateOrderButton_Click(object sender, RoutedEventArgs e)
        {
            var orderWindow = new CreateOrderWindow(_context, _currentUser);
            orderWindow.ShowDialog();

            if (!IsAdmin())
                LoadOrdersPublic();
            else
                LoadAdminOrders();
        }
        #endregion

        #region Logout
        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
        #endregion

        #region Admin Boekenbeheer
        private void LoadAdminBooks()
        {
            if (!IsAdmin()) return;
            AdminBooksDataGrid.ItemsSource = _context.Books.Where(b => !b.IsDeleted).ToList();
        }

        private void AddBookButton_Click(object sender, RoutedEventArgs e)
        {
            if (!IsAdmin()) { ShowAdminAccessError(); return; }
            var add = new AddBookWindow(_context);
            add.ShowDialog();
            LoadCatalogus();
            LoadAdminBooks();
        }

        private void EditBookButton_Click(object sender, RoutedEventArgs e)
        {
            if (!IsAdmin()) { ShowAdminAccessError(); return; }
            if (AdminBooksDataGrid.SelectedItem is not Book selected) { ShowSelectItemWarning("boek"); return; }
            var edit = new EditBookWindow(_context, selected);
            edit.ShowDialog();
            LoadAdminBooks();
        }

        private void DeleteBookButton_Click(object sender, RoutedEventArgs e)
        {
            if (!IsAdmin()) { ShowAdminAccessError(); return; }
            if (AdminBooksDataGrid.SelectedItem is not Book selected) { ShowSelectItemWarning("boek"); return; }

            if (MessageBox.Show($"Weet je zeker dat je '{selected.Title}' wilt verwijderen?", "Bevestigen", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                selected.IsDeleted = true;
                _context.Books.Update(selected);
                _context.SaveChanges();
                LoadAdminBooks();
                LoadCatalogus();
            }
        }
        #endregion

        #region Admin Ordersbeheer
        private void LoadAdminOrders()
        {
            if (!IsAdmin()) return;

            var orders = _context.Orders
                .Include(o => o.User)
                .Include(o => o.Books)
                .Where(o => !o.IsDeleted)
                .ToList();

            _adminOrders = orders.Select(o => new AdminOrderView
            {
                Id = o.Id,
                UserName = o.User != null ? o.User.UserName : "Onbekend",
                BookTitles = string.Join(", ", o.Books.Select(b => b.Title)),
                TotalPrice = o.TotalPrice,
                IsProcessed = o.IsProcessed
            }).ToList();

            AdminOrdersDataGrid.ItemsSource = _adminOrders;
        }


        private void MarkProcessedButton_Click(object sender, RoutedEventArgs e)
        {
            if (!IsAdmin()) { ShowAdminAccessError(); return; }
            if (AdminOrdersDataGrid.SelectedItem is not AdminOrderView selected) { ShowSelectItemWarning("bestelling"); return; }

            var orderEntity = _context.Orders.FirstOrDefault(o => o.Id == selected.Id);
            if (orderEntity != null)
            {
                orderEntity.IsProcessed = true;
                _context.SaveChanges();
                LoadAdminOrders();
                MessageBox.Show($"Bestelling {orderEntity.Id} gemarkeerd als verwerkt.", "Gereed", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void RefreshOrdersButton_Click(object sender, RoutedEventArgs e)
        {
            if (!IsAdmin()) return;
            LoadAdminOrders();
            MessageBox.Show("Bestellingen vernieuwd.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        #endregion

        #region Admin Zoeken
        private void AdminSearchTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            string query = AdminSearchTextBox.Text?.Trim() ?? string.Empty;
            AdminPlaceholder.Visibility = string.IsNullOrEmpty(query) ? Visibility.Visible : Visibility.Hidden;

            if (string.IsNullOrEmpty(query))
            {
                LoadAdminBooks();
                LoadAdminOrders();
                return;
            }

            AdminBooksDataGrid.ItemsSource = _context.Books
                .Where(b => !b.IsDeleted &&
                            (b.Title.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                             b.Author.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                             b.ISBN.Contains(query, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            AdminOrdersDataGrid.ItemsSource = _adminOrders
                .Where(o => o.UserName.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                            o.BookTitles.Contains(query, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
        #endregion

        #region Publieke Bestellingen
        private void LoadOrdersPublic()
        {
            var orders = _context.Orders
                .Include(o => o.User)
                .Include(o => o.Books)
                .Where(o => !o.IsDeleted && o.UserId == _currentUser.Id)
                .ToList();

            OrdersDataGrid.ItemsSource = orders.Select(o => new
            {
                o.Id,
                UserName = o.User != null ? o.User.UserName : "Onbekend",
                BookTitles = string.Join(", ", o.Books.Select(b => b.Title)),
                o.TotalPrice,
                OrderDate = o.OrderDate.ToString("dd/MM/yyyy HH:mm")
            }).ToList();
        }
        #endregion

        #region Gebruikersbeheer
        private void LoadUsers()
        {
            if (!IsAdmin()) return;
            _users = _context.Users.ToList();
            UsersDataGrid.ItemsSource = _users;
        }

        private void UsersSearchTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            string query = UsersSearchTextBox.Text?.Trim() ?? string.Empty;
            UsersPlaceholder.Visibility = string.IsNullOrEmpty(query) ? Visibility.Visible : Visibility.Hidden;

            UsersDataGrid.ItemsSource = _users
                .Where(u => u.UserName.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                            (u.Email != null && u.Email.Contains(query, StringComparison.OrdinalIgnoreCase)))
                .ToList();
        }

        private void DeleteUserButton_Click(object sender, RoutedEventArgs e)
        {
            if (!IsAdmin()) { ShowAdminAccessError(); return; }
            if (UsersDataGrid.SelectedItem is not ApplicationUser selected) { ShowSelectItemWarning("gebruiker"); return; }
            if (selected.Id == _currentUser.Id) { MessageBox.Show("Je kunt jezelf niet verwijderen.", "Fout", MessageBoxButton.OK, MessageBoxImage.Warning); return; }

            if (MessageBox.Show($"Weet je zeker dat je '{selected.UserName}' wilt verwijderen?", "Bevestigen", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                _context.Users.Remove(selected);
                _context.SaveChanges();
                LoadUsers();
            }
        }

        private void RoleComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (!IsAdmin()) return;

            if (sender is System.Windows.Controls.ComboBox comboBox &&
                comboBox.SelectedItem is System.Windows.Controls.ComboBoxItem selectedItem &&
                UsersDataGrid.SelectedItem is ApplicationUser user)
            {
                string newRole = selectedItem.Content?.ToString() ?? string.Empty;
                if (string.IsNullOrWhiteSpace(newRole)) return;

                var roleEntity = _context.Roles.FirstOrDefault(r => r.Name == newRole);
                if (roleEntity == null) { MessageBox.Show("Ongeldige rol geselecteerd.", "Fout", MessageBoxButton.OK, MessageBoxImage.Error); return; }

                var userRole = _context.UserRoles.FirstOrDefault(ur => ur.UserId == user.Id);
                if (userRole != null) { userRole.RoleId = roleEntity.Id; _context.UserRoles.Update(userRole); }
                else { _context.UserRoles.Add(new IdentityUserRole<string> { UserId = user.Id, RoleId = roleEntity.Id }); }

                _context.SaveChanges();
                LoadUsers();
                MessageBox.Show($"Rol van gebruiker '{user.UserName}' gewijzigd naar '{newRole}'.", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        #endregion

        #region Contacteer Admin
        private void SendContactMessageButton_Click(object sender, RoutedEventArgs e)
        {
            string subject = ContactSubjectTextBox.Text.Trim();
            string message = ContactMessageTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(subject) || string.IsNullOrWhiteSpace(message))
            {
                MessageBox.Show("Vul zowel onderwerp als bericht in.", "Let op", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var contactMessage = new ContactMessage
            {
                UserId = _currentUser.Id,
                UserName = _currentUser.UserName,
                Subject = subject,
                Message = message,
                CreatedAt = DateTime.Now,
                IsResolved = false
            };

            _context.ContactMessages.Add(contactMessage);
            _context.SaveChanges();

            ContactStatusTextBlock.Text = "✅ Bericht verstuurd!";
            ContactSubjectTextBox.Clear();
            ContactMessageTextBox.Clear();
        }
        #endregion

        #region Admin Contact Messages
        private void LoadContactMessages()
        {
            if (!IsAdmin()) return;

            _contactMessages = _context.ContactMessages
                .Include(m => m.User)
                .OrderByDescending(m => m.CreatedAt)
                .ToList();

            AdminContactMessagesDataGrid.ItemsSource = _contactMessages;
        }

        private void ContactAdminSearchTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            string query = ContactAdminSearchTextBox.Text?.Trim() ?? string.Empty;
            ContactAdminPlaceholder.Visibility = string.IsNullOrEmpty(query) ? Visibility.Visible : Visibility.Hidden;

            AdminContactMessagesDataGrid.ItemsSource = string.IsNullOrEmpty(query)
                ? _contactMessages
                : _contactMessages.Where(m =>
                    (m.UserName ?? "").Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    m.Subject.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    m.Message.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        private void MarkContactResolvedButton_Click(object sender, RoutedEventArgs e)
        {
            if (!IsAdmin()) return;
            if (AdminContactMessagesDataGrid.SelectedItem is not ContactMessage selected) { ShowSelectItemWarning("contactbericht"); return; }

            selected.IsResolved = true;
            _context.SaveChanges();
            LoadContactMessages();
        }

        private void DeleteContactMessageButton_Click(object sender, RoutedEventArgs e)
        {
            if (!IsAdmin()) return;
            if (AdminContactMessagesDataGrid.SelectedItem is not ContactMessage selected) { ShowSelectItemWarning("contactbericht"); return; }

            if (MessageBox.Show($"Weet je zeker dat je dit bericht wilt verwijderen?", "Bevestigen", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                _context.ContactMessages.Remove(selected);
                _context.SaveChanges();
                LoadContactMessages();
            }
        }

        private void RefreshContactMessagesButton_Click(object sender, RoutedEventArgs e)
        {
            LoadContactMessages();
            MessageBox.Show("Contactberichten vernieuwd.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        #endregion

        #region Helpers
        private void ShowAdminAccessError() => MessageBox.Show("Alleen admins kunnen deze actie uitvoeren.", "Geen toegang", MessageBoxButton.OK, MessageBoxImage.Warning);
        private void ShowSelectItemWarning(string item) => MessageBox.Show($"Selecteer eerst een {item}.", "Let op", MessageBoxButton.OK, MessageBoxImage.Warning);
        #endregion
    }
}
