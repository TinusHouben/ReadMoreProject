using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ReadMore.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public string Author { get; set; } = string.Empty;

        public string ISBN { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public bool IsDeleted { get; set; }

        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
