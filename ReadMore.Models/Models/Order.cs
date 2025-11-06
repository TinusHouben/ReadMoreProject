namespace ReadMore.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int? BookId { get; set; }
        public int? ComicId { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public bool IsDeleted { get; set; }
    }
}
