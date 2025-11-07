using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using ReadMore.Models;
using Microsoft.EntityFrameworkCore;

namespace ReadMore.WPF
{
    public partial class MainWindow : Window
    {
        private readonly ApplicationDbContext _context;
        private readonly ApplicationUser _currentUser;
        private List<Book> _books = new List<Book>();
        private List<Order> _orders = new List<Order>();

        public MainWindow(ApplicationDbContext context, ApplicationUser user)
        {
            InitializeComponent();
            _context = context;
            _currentUser = user;

            LoadCatalogus();
            LoadOrders();
            LoadAdminOrders();
        }

        private void LoadCatalogus()
        {
            _books = _context.Books
                .Where(b => !b.IsDeleted)
                .ToList();

            CatalogusDataGrid.ItemsSource = _books;
        }

        private void LoadOrders()
        {
            _orders = _context.Orders
                .Include(o => o.User)
                .Include(o => o.Books)
                .Where(o => !o.IsDeleted)
                .ToList();

            OrdersDataGrid.ItemsSource = _orders.Select(o => new
            {
                o.Id,
                UserName = o.User?.UserName ?? "Onbekend",
                BookTitles = string.Join(", ", o.Books.Select(b => b.Title)),
                o.TotalPrice,
                o.OrderDate
            }).ToList();
        }

        private void LoadAdminOrders()
        {
            var adminOrders = _context.Orders
                .Include(o => o.User)
                .Include(o => o.Books)
                .Where(o => !o.IsDeleted)
                .ToList();

            AdminOrdersDataGrid.ItemsSource = adminOrders.Select(o => new AdminOrderView
            {
                Id = o.Id,
                UserName = o.User?.UserName ?? "Onbekend",
                BookTitles = string.Join(", ", o.Books.Select(b => b.Title)),
                TotalPrice = o.TotalPrice,
                IsProcessed = o.IsProcessed
            }).ToList();
        }

        private void CatalogusSearchTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            var filtered = _books
                .Where(b => b.Title.IndexOf(CatalogusSearchTextBox.Text, StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();

            CatalogusDataGrid.ItemsSource = filtered;
        }

        private void AdminSearchTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            var filtered = _context.Books
                .AsEnumerable() // client-side filtering
                .Where(b => b.Title.Contains(AdminSearchTextBox.Text, StringComparison.OrdinalIgnoreCase) && !b.IsDeleted)
                .ToList();

            AdminOrdersDataGrid.ItemsSource = filtered.Select(b => new AdminOrderView
            {
                Id = b.Id.ToString(),
                UserName = "",
                BookTitles = b.Title,
                TotalPrice = b.Price,
                IsProcessed = false
            }).ToList();
        }

        private void CreateOrderButton_Click(object sender, RoutedEventArgs e)
        {
            var orderWindow = new CreateOrderWindow(_context, _currentUser);
            orderWindow.ShowDialog();
            LoadOrders();
            LoadAdminOrders();
        }

        private void AddBookButton_Click(object sender, RoutedEventArgs e)
        {
            var addBookWindow = new AddBookWindow(_context);
            addBookWindow.ShowDialog();
            LoadCatalogus();
            LoadAdminOrders();
        }

        private void EditBookButton_Click(object sender, RoutedEventArgs e) { }
        private void DeleteBookButton_Click(object sender, RoutedEventArgs e) { }
    }
}
