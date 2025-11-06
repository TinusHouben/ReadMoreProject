using ReadMore.Models;
using System.Windows;
using System.Windows.Controls;

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
            PriceTextBox.Text = _book.Price.ToString();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            _book.Title = TitleTextBox.Text;
            _book.Author = AuthorTextBox.Text;
            _book.Price = decimal.Parse(PriceTextBox.Text);

            _context.SaveChanges();
            this.Close();
        }
    }
}
