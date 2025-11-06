using System.Linq;
using System.Windows;
using ReadMore.Models;
using Microsoft.EntityFrameworkCore;

namespace ReadMore.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ApplicationDbContext _context;

        public MainWindow()
        {
            InitializeComponent();

            // DbContext instellen
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=ReadMoreDb;Trusted_Connection=True;")
                .Options;

            _context = new ApplicationDbContext(options);

            // DataGrid vullen bij opstarten
            LoadBooks();
        }

        /// <summary>
        /// Haalt boeken op uit de database en toont ze in de DataGrid
        /// </summary>
        private void LoadBooks()
        {
            var books = _context.Books
                                .Where(b => !b.IsDeleted) // Alleen niet-verwijderde boeken
                                .ToList();

            BooksDataGrid.ItemsSource = books;
        }

        /// <summary>
        /// Add Book button click
        /// </summary>
        private void AddBookButton_Click(object sender, RoutedEventArgs e)
        {
            var addBookWindow = new AddBookWindow(_context);
            addBookWindow.ShowDialog();
            LoadBooks(); // DataGrid verversen
        }

        /// <summary>
        /// Edit Book button click
        /// </summary>
        private void EditBookButton_Click(object sender, RoutedEventArgs e)
        {
            if (BooksDataGrid.SelectedItem is Book selectedBook)
            {
                var editBookWindow = new EditBookWindow(_context, selectedBook);
                editBookWindow.ShowDialog();
                LoadBooks(); // DataGrid verversen
            }
            else
            {
                MessageBox.Show("Selecteer eerst een boek om te bewerken.",
                                "Waarschuwing", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// Delete Book button click (soft delete)
        /// </summary>
        private void DeleteBookButton_Click(object sender, RoutedEventArgs e)
        {
            if (BooksDataGrid.SelectedItem is Book selectedBook)
            {
                var result = MessageBox.Show($"Weet je zeker dat je '{selectedBook.Title}' wilt verwijderen?",
                                             "Bevestigen", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    selectedBook.IsDeleted = true; // Soft delete
                    _context.SaveChanges();
                    LoadBooks(); // DataGrid verversen
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
