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
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var book = new Book
            {
                Title = TitleTextBox.Text,
                Author = AuthorTextBox.Text,
                ISBN = IsbnTextBox.Text,
                Price = decimal.TryParse(PriceTextBox.Text, out var price) ? price : 0
            };

            _context.Books.Add(book);
            _context.SaveChanges();
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
