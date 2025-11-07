using ReadMore.Models;
using System.Windows;

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
                Price = decimal.Parse(PriceTextBox.Text),
                IsDeleted = false
            };

            _context.Books.Add(book);
            _context.SaveChanges();
            this.Close();
        }
    }
}
