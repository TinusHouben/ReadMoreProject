using System.Linq;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using ReadMore.Models;

namespace ReadMore.WPF
{
    public partial class MainWindow : Window
    {
        private readonly ApplicationDbContext _context;
        private readonly ApplicationUser _user;

        public MainWindow(ApplicationDbContext context, ApplicationUser user)
        {
            InitializeComponent();
            _context = context;
            _user = user;

            LoadBooks();
            LoadOrders();
        }

        private void LoadBooks()
        {
            var books = _context.Books.Where(b => !b.IsDeleted).ToList();
            BooksDataGrid.ItemsSource = books;
        }

        private void LoadOrders()
        {
            var orders = _context.Orders
                .Include(o => o.Books)
                .Include(o => o.User)
                .Where(o => !o.IsDeleted)
                .ToList();

            var displayOrders = orders.Select(o => new
            {
                o.Id,
                UserName = o.User?.UserName ?? "Onbekend",
                BookTitles = string.Join(", ", o.Books.Select(b => b.Title)),
                o.TotalPrice,
                o.OrderDate
            }).ToList();

            OrdersDataGrid.ItemsSource = displayOrders;
        }

        private void CreateOrderButton_Click(object sender, RoutedEventArgs e)
        {
            var orderWindow = new CreateOrderWindow(_context, _user);
            orderWindow.ShowDialog();
            LoadOrders();
        }

        private void AddBookButton_Click(object sender, RoutedEventArgs e) { }
        private void EditBookButton_Click(object sender, RoutedEventArgs e) { }
        private void DeleteBookButton_Click(object sender, RoutedEventArgs e) { }
        private void SearchTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) { }
    }
}
