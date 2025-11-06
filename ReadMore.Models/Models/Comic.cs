namespace ReadMore.Models
{
    public class Comic
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Series { get; set; } = string.Empty;
        public int NumberInSeries { get; set; } 
        public decimal Price { get; set; }
        public bool IsDeleted { get; set; }
    }
}
