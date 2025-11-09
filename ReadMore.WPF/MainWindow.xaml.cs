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

        public MainWindow(ApplicationDbContext context, ApplicationUser user)
        {
            InitializeComponent();
            _context = context;
            _currentUser = user;

            ConfigureAccess();
            LoadCatalogus();
            LoadOrdersPublic();

            // Alleen laden als admin
            if (IsAdmin())
            {
                LoadAdminBooks();
                LoadAdminOrders();
                LoadUsers();
            }
        }

        // 🔒 Controleer of de gebruiker admin is
        private bool IsAdmin()
        {
            return _context.UserRoles
                .Any(ur => ur.UserId == _currentUser.Id &&
                           _context.Roles.Any(r => r.Id == ur.RoleId && r.Name == "Admin"));
        }

        private void ConfigureAccess()
        {
            bool isAdmin = IsAdmin();
            Title = $"📚 ReadMore - Ingelogd als {_currentUser.UserName} {(isAdmin ? "(Admin)" : "(Gebruiker)")}";

            if (!isAdmin)
            {
                RemoveTab("Admin"); // alleen boeken
                RemoveTab("Bestellingenbeheer"); // admin orders
                RemoveTab("Gebruikersbeheer");
            }
        }


        private void RemoveTab(string headerContains)
        {
            var tab = MainTabControl.Items
                .OfType<System.Windows.Controls.TabItem>()
                .FirstOrDefault(t => t.Header.ToString().Contains(headerContains));
            if (tab != null)
                MainTabControl.Items.Remove(tab);
        }

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

            var filtered = _catalogBooks
                .Where(b => b.Title.Contains(q, StringComparison.OrdinalIgnoreCase))
                .ToList();

            CatalogusDataGrid.ItemsSource = filtered;
        }

        private void CreateOrderButton_Click(object sender, RoutedEventArgs e)
        {
            var orderWindow = new CreateOrderWindow(_context, _currentUser);
            orderWindow.ShowDialog();
            LoadOrdersPublic();
            if (IsAdmin()) LoadAdminOrders();
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

        #region Admin functies
        private void LoadAdminBooks()
        {
            if (!IsAdmin()) return;
            AdminBooksDataGrid.ItemsSource = _context.Books.Where(b => !b.IsDeleted).ToList();
        }

        private void AddBookButton_Click(object sender, RoutedEventArgs e)
        {
            if (!IsAdmin())
            {
                MessageBox.Show("Alleen admins kunnen boeken toevoegen.", "Geen toegang",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var add = new AddBookWindow(_context);
            add.ShowDialog();
            LoadCatalogus();
            LoadAdminBooks();
        }

        private void EditBookButton_Click(object sender, RoutedEventArgs e)
        {
            if (!IsAdmin())
            {
                MessageBox.Show("Alleen admins kunnen boeken bewerken.", "Geen toegang",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (AdminBooksDataGrid.SelectedItem is not Book selected)
            {
                MessageBox.Show("Selecteer eerst een boek.", "Let op",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var edit = new EditBookWindow(_context, selected);
            edit.ShowDialog();
            LoadAdminBooks();
        }

        private void DeleteBookButton_Click(object sender, RoutedEventArgs e)
        {
            if (!IsAdmin())
            {
                MessageBox.Show("Alleen admins kunnen boeken verwijderen.", "Geen toegang",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (AdminBooksDataGrid.SelectedItem is not Book selected)
            {
                MessageBox.Show("Selecteer eerst een boek.", "Let op",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show($"Weet je zeker dat je '{selected.Title}' wilt verwijderen?",
                "Bevestigen", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                selected.IsDeleted = true;
                _context.Books.Update(selected);
                _context.SaveChanges();
                LoadAdminBooks();
                LoadCatalogus();
            }
        }

        private void LoadAdminOrders()
        {
            if (!IsAdmin()) return;

            _adminOrders = _context.Orders
                .Include(o => o.User)
                .Include(o => o.Books)
                .Where(o => !o.IsDeleted)
                .Select(o => new AdminOrderView
                {
                    Id = o.Id,
                    UserName = o.User != null ? o.User.UserName : "Onbekend",
                    BookTitles = string.Join(", ", o.Books.Select(b => b.Title)),
                    TotalPrice = o.TotalPrice,
                    IsProcessed = o.IsProcessed
                })
                .ToList();

            AdminOrdersDataGrid.ItemsSource = _adminOrders;
        }

        private void MarkProcessedButton_Click(object sender, RoutedEventArgs e)
        {
            if (!IsAdmin())
            {
                MessageBox.Show("Alleen admins kunnen bestellingen verwerken.", "Geen toegang",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (AdminOrdersDataGrid.SelectedItem is not AdminOrderView selected)
            {
                MessageBox.Show("Selecteer een bestelling.", "Let op",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var orderEntity = _context.Orders.FirstOrDefault(o => o.Id == selected.Id);
            if (orderEntity != null)
            {
                orderEntity.IsProcessed = true;
                _context.SaveChanges();
                LoadAdminOrders();
                MessageBox.Show($"Bestelling {orderEntity.Id} gemarkeerd als verwerkt.", "Gereed",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        #endregion

        #region Admin zoeken en vernieuwen
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

            var filteredBooks = _context.Books
                .Where(b => !b.IsDeleted &&
                            (b.Title.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                             b.Author.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                             b.ISBN.Contains(query, StringComparison.OrdinalIgnoreCase)))
                .ToList();
            AdminBooksDataGrid.ItemsSource = filteredBooks;

            var filteredOrders = _adminOrders
                .Where(o => o.UserName.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                            o.BookTitles.Contains(query, StringComparison.OrdinalIgnoreCase))
                .ToList();
            AdminOrdersDataGrid.ItemsSource = filteredOrders;
        }

        private void RefreshOrdersButton_Click(object sender, RoutedEventArgs e)
        {
            if (!IsAdmin()) return;
            LoadAdminOrders();
            MessageBox.Show("Bestellingen vernieuwd.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        #endregion

        #region Publieke bestellingen
        private void LoadOrdersPublic()
        {
            var orders = _context.Orders
                .Include(o => o.User)
                .Include(o => o.Books)
                .Where(o => !o.IsDeleted)
                .ToList();

            OrdersDataGrid.ItemsSource = orders.Select(o => new
            {
                o.Id,
                UserName = o.User?.UserName ?? "Onbekend",
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

            var filtered = _users
                .Where(u => u.UserName.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                            (u.Email != null && u.Email.Contains(query, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            UsersDataGrid.ItemsSource = filtered;
        }

        private void DeleteUserButton_Click(object sender, RoutedEventArgs e)
        {
            if (!IsAdmin())
            {
                MessageBox.Show("Alleen admins kunnen gebruikers verwijderen.", "Geen toegang",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (UsersDataGrid.SelectedItem is not ApplicationUser selected)
            {
                MessageBox.Show("Selecteer een gebruiker.", "Let op",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (selected.Id == _currentUser.Id)
            {
                MessageBox.Show("Je kunt jezelf niet verwijderen.", "Fout",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show($"Weet je zeker dat je '{selected.UserName}' wilt verwijderen?",
                "Bevestigen", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
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
                if (roleEntity == null)
                {
                    MessageBox.Show("Ongeldige rol geselecteerd.", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var userRole = _context.UserRoles.FirstOrDefault(ur => ur.UserId == user.Id);
                if (userRole != null)
                {
                    userRole.RoleId = roleEntity.Id;
                    _context.UserRoles.Update(userRole);
                }
                else
                {
                    _context.UserRoles.Add(new IdentityUserRole<string>
                    {
                        UserId = user.Id,
                        RoleId = roleEntity.Id
                    });
                }

                _context.SaveChanges();
                LoadUsers(); // ✅ lijst vernieuwen zonder herstart
                MessageBox.Show($"Rol van gebruiker '{user.UserName}' is gewijzigd naar '{newRole}'.", "Succes",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        #endregion
    }
}
