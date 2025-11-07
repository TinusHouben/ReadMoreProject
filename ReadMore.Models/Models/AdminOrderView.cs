namespace ReadMore.Models
{
    public class AdminOrderView
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string BookTitles { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }
        public bool IsProcessed { get; set; }
    }
}
