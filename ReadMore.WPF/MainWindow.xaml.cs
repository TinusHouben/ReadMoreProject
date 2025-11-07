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
        private List<Book> _booksList = new List<Book>();
        private List<Order> _orders = new List<Order>();

        public MainWindow(ApplicationDbContext context, ApplicationUser user)
        {
            InitializeComponent();
            _context = context;
            _currentUser = user;

            LoadBooks();
            LoadOrders();
        }

        private void LoadBooks()
        {
            _booksList = _context.Books
                .Where(b => !b.IsDeleted)
                .ToList();

            BooksDataGrid.ItemsSource = _booksList;
            AdminBooksDataGrid.ItemsSource = _booksList;
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

        private void SearchCatalogTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            var filtered = _booksList
                .Where(b => b.Title.Contains(SearchCatalogTextBox.Text, StringComparison.OrdinalIgnoreCase))
                .ToList();

            BooksDataGrid.ItemsSource = filtered;
        }

        private void SearchAdminTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            var filtered = _booksList
                .Where(b => b.Title.Contains(SearchAdminTextBox.Text, StringComparison.OrdinalIgnoreCase))
                .ToList();

            AdminBooksDataGrid.ItemsSource = filtered;
        }

        private void CreateOrderButton_Click(object sender, RoutedEventArgs e)
        {
            var orderWindow = new CreateOrderWindow(_context, _currentUser);
            orderWindow.ShowDialog();
            LoadOrders();
        }

        private void AddBookButton_Click(object sender, RoutedEventArgs e) { }
        private void EditBookButton_Click(object sender, RoutedEventArgs e) { }
        private void DeleteBookButton_Click(object sender, RoutedEventArgs e) { }
    }
}
