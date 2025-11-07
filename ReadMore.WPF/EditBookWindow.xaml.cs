using System.Windows;
using ReadMore.Models;

namespace ReadMore.WPF
{
    public partial class EditBookWindow : Window
    {
        private readonly ApplicationDbContext _context;
        private readonly Book _book;

        public EditBookWindow(ApplicationDbContext context, Book book)
        {
            InitializeComponent();
            _context = context;
            _book = book;

            TitleTextBox.Text = _book.Title;
            AuthorTextBox.Text = _book.Author;
            IsbnTextBox.Text = _book.ISBN;
            PriceTextBox.Text = _book.Price.ToString("0.00");
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            _book.Title = TitleTextBox.Text;
            _book.Author = AuthorTextBox.Text;
            _book.ISBN = IsbnTextBox.Text;

            if (decimal.TryParse(PriceTextBox.Text, out decimal price))
                _book.Price = price;

            _context.SaveChanges();
            Close();
        }
    }
}
