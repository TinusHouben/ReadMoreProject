using System;
using System.Collections.Generic;

namespace ReadMore.Models
{
    public class Order
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = new ApplicationUser();
        public List<Book> Books { get; set; } = new List<Book>();
        public decimal TotalPrice { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public bool IsDeleted { get; set; }

        public bool IsProcessed { get; set; } = false;

    }
}
