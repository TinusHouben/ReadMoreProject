using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using ReadMore.Models;

namespace ReadMore.WPF
{
    public partial class MainWindow : Window
    {
        private readonly ApplicationDbContext _context;
        private readonly ApplicationUser _currentUser;

        private List<Book> _catalogBooks = new();
        private List<AdminOrderView> _adminOrders = new();

        public MainWindow(ApplicationDbContext context, ApplicationUser user)
        {
            InitializeComponent();
            _context = context;
            _currentUser = user;

            LoadCatalogus();
            LoadAdminBooks();
            LoadAdminOrders();
            LoadOrdersPublic();
        }

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
            LoadAdminOrders();
            LoadOrdersPublic();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        private void LoadAdminBooks()
        {
            var books = _context.Books.Where(b => !b.IsDeleted).ToList();
            AdminBooksDataGrid.ItemsSource = books;
        }

        private void AddBookButton_Click(object sender, RoutedEventArgs e)
        {
            var add = new AddBookWindow(_context);
            add.ShowDialog();
            LoadCatalogus();
            LoadAdminBooks();
        }

        private void EditBookButton_Click(object sender, RoutedEventArgs e)
        {
            Book selected = AdminBooksDataGrid.SelectedItem as Book;

            if (selected == null)
            {
                MessageBox.Show("Selecteer eerst een boek om te bewerken.", "Fout", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var edit = new EditBookWindow(_context, selected);
            edit.ShowDialog();
            _context.SaveChanges();
            LoadCatalogus();
            LoadAdminBooks();
        }

        private void DeleteBookButton_Click(object sender, RoutedEventArgs e)
        {
            Book selected = AdminBooksDataGrid.SelectedItem as Book;

            if (selected == null)
            {
                MessageBox.Show("Selecteer eerst een boek om te verwijderen.", "Fout", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var ok = MessageBox.Show($"Weet je zeker dat je '{selected.Title}' wilt verwijderen?", "Bevestigen", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (ok == MessageBoxResult.Yes)
            {
                selected.IsDeleted = true;
                _context.Books.Update(selected);
                _context.SaveChanges();
                LoadCatalogus();
                LoadAdminBooks();
            }
        }

        private void LoadAdminOrders()
        {
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

        private void AdminSearchTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            string q = AdminSearchTextBox.Text?.Trim() ?? string.Empty;
            AdminPlaceholder.Visibility = string.IsNullOrEmpty(q) ? Visibility.Visible : Visibility.Hidden;

            if (string.IsNullOrEmpty(q))
            {
                AdminBooksDataGrid.ItemsSource = _context.Books.Where(b => !b.IsDeleted).ToList();
                AdminOrdersDataGrid.ItemsSource = _adminOrders;
                return;
            }

            var booksFiltered = _context.Books
                .AsEnumerable()
                .Where(b => !b.IsDeleted && (b.Title.Contains(q, StringComparison.OrdinalIgnoreCase) || b.Author.Contains(q, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            var ordersFiltered = _adminOrders
                .Where(o => o.UserName.Contains(q, StringComparison.OrdinalIgnoreCase) || o.BookTitles.Contains(q, StringComparison.OrdinalIgnoreCase))
                .ToList();

            AdminBooksDataGrid.ItemsSource = booksFiltered;
            AdminOrdersDataGrid.ItemsSource = ordersFiltered;
        }

        private void RefreshOrdersButton_Click(object sender, RoutedEventArgs e)
        {
            LoadAdminOrders();
            LoadOrdersPublic();
        }

        private void MarkProcessedButton_Click(object sender, RoutedEventArgs e)
        {
            if (AdminOrdersDataGrid.SelectedItem is not AdminOrderView selected)
            {
                MessageBox.Show("Selecteer eerst een bestelling die je wilt markeren.", "Let op", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var orderEntity = _context.Orders.FirstOrDefault(o => o.Id == selected.Id);
            if (orderEntity != null)
            {
                orderEntity.IsProcessed = true;
                _context.SaveChanges();
                LoadAdminOrders();
                MessageBox.Show($"Bestelling {orderEntity.Id} gemarkeerd als verwerkt.", "Gereed", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

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
    }
}
