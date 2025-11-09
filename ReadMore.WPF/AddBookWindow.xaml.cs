using System;
using System.Windows;
using ReadMore.Models;

namespace ReadMore.WPF
{
    public partial class AddBookWindow : Window
    {
        private readonly ApplicationDbContext _context;

        public AddBookWindow(ApplicationDbContext context)
        {
            InitializeComponent();
            _context = context;

            TitleTextBox.Focus();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string title = TitleTextBox.Text.Trim();
            string author = AuthorTextBox.Text.Trim();
            string isbn = IsbnTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(title) ||
                string.IsNullOrWhiteSpace(author) ||
                string.IsNullOrWhiteSpace(isbn) ||
                !decimal.TryParse(PriceTextBox.Text.Trim(), out var price))
            {
                MessageBox.Show("Vul alle velden correct in, inclusief een geldig prijsbedrag.",
                                "Ongeldige invoer", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var book = new Book
            {
                Title = title,
                Author = author,
                ISBN = isbn,
                Price = price
            };

            _context.Books.Add(book);
            _context.SaveChanges();

            MessageBox.Show($"Boek '{title}' succesvol toegevoegd.", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
