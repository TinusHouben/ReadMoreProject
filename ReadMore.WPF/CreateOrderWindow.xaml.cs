using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using ReadMore.Models;

namespace ReadMore.WPF
{
    public partial class CreateOrderWindow : Window
    {
        private readonly ApplicationDbContext _context;
        private readonly ApplicationUser _user;
        private List<OrderBookItem> _booksList;

        public CreateOrderWindow(ApplicationDbContext context, ApplicationUser user)
        {
            InitializeComponent();
            _context = context;
            _user = user;
            LoadBooks();
        }

        private void LoadBooks()
        {
            _booksList = _context.Books
                .Where(b => !b.IsDeleted)
                .Select(b => new OrderBookItem
                {
                    BookId = b.Id,
                    Title = b.Title,
                    Price = b.Price,
                    Quantity = 0
                })
                .ToList();

            BooksDataGrid.ItemsSource = _booksList;
            UpdateTotal();
        }

        private void UpdateTotal()
        {
            decimal total = _booksList.Sum(b => b.Price * b.Quantity);
            TotalTextBlock.Text = total.ToString("0.00");
        }

        private void ConfirmOrderButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedBooks = _booksList.Where(b => b.Quantity > 0).ToList();

            if (!selectedBooks.Any())
            {
                MessageBox.Show("Selecteer minstens één boek om te bestellen.", "Fout", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var order = new Order
            {
                UserId = _user.Id,
                User = _user,
                OrderDate = DateTime.Now,
                TotalPrice = selectedBooks.Sum(b => b.Price * b.Quantity),
                Books = selectedBooks.Select(b => _context.Books.Find(b.BookId)!).ToList()
            };

            _context.Orders.Add(order);
            _context.SaveChanges();

            MessageBox.Show("Bestelling succesvol geplaatst!", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private class OrderBookItem
        {
            public int BookId { get; set; }
            public string Title { get; set; } = string.Empty;
            public decimal Price { get; set; }
            public int Quantity { get; set; }
        }
    }
}
