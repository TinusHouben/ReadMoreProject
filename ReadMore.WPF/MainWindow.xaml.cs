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

        public MainWindow(ApplicationDbContext context, ApplicationUser currentUser)
        {
            InitializeComponent();

            _context = context;
            _currentUser = currentUser;

            LoadBooks();

            // Alleen admin kan knoppen gebruiken
            bool isAdmin = _context.UserRoles
                                    .Any(ur => ur.UserId == _currentUser.Id && ur.RoleId == "1"); // "1" = Admin rol

            AddBookButton.IsEnabled = isAdmin;
            EditBookButton.IsEnabled = isAdmin;
            DeleteBookButton.IsEnabled = isAdmin;
        }

        private void LoadBooks()
        {
            var books = _context.Books
                                .Where(b => !b.IsDeleted)
                                .ToList();
            BooksDataGrid.ItemsSource = books;
        }

        private void AddBookButton_Click(object sender, RoutedEventArgs e)
        {
            var addBookWindow = new AddBookWindow(_context);
            addBookWindow.ShowDialog();
            LoadBooks();
        }

        private void EditBookButton_Click(object sender, RoutedEventArgs e)
        {
            if (BooksDataGrid.SelectedItem is Book selectedBook)
            {
                var editBookWindow = new EditBookWindow(_context, selectedBook);
                editBookWindow.ShowDialog();
                LoadBooks();
            }
            else
            {
                MessageBox.Show("Selecteer eerst een boek om te bewerken.",
                                "Waarschuwing", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeleteBookButton_Click(object sender, RoutedEventArgs e)
        {
            if (BooksDataGrid.SelectedItem is Book selectedBook)
            {
                var result = MessageBox.Show($"Weet je zeker dat je '{selectedBook.Title}' wilt verwijderen?",
                                             "Bevestigen", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    selectedBook.IsDeleted = true;
                    _context.SaveChanges();
                    LoadBooks();
                }
            }
            else
            {
                MessageBox.Show("Selecteer eerst een boek om te verwijderen.",
                                "Waarschuwing", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
