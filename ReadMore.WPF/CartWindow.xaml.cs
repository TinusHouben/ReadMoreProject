using System.Collections.Generic;
using System.Linq;
using System.Windows;
using ReadMore.Models;

namespace ReadMore.WPF
{
    public partial class CartWindow : Window
    {
        private List<Book> _cartBooks;

        public CartWindow(List<Book> cartBooks)
        {
            InitializeComponent();
            _cartBooks = cartBooks;
            CartDataGrid.ItemsSource = _cartBooks;
            UpdateTotalPrice();
        }

        private void SearchTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            string filter = SearchTextBox.Text.ToLower();
            CartDataGrid.ItemsSource = _cartBooks
                .Where(b => b.Title.ToLower().Contains(filter) || b.Author.ToLower().Contains(filter))
                .ToList();
        }

        private void PlaceOrderButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Bestelling geplaatst!");
        }

        private void UpdateTotalPrice()
        {
            TotalPriceText.Text = _cartBooks.Sum(b => b.Price).ToString("C");
        }
    }
}
