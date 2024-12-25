using System;
using System.Collections.Generic;

namespace PetStore.Models
{
    public partial class Product
    {
        public Product()
        {
            Feedbacks = new HashSet<Feedback>();
            ProductDetails = new HashSet<ProductDetail>();
        }

        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? Details { get; set; }
        public decimal? Price { get; set; }
        public string? Image { get; set; }
        public double? Discount { get; set; }
        public string? Status { get; set; }
        public int? CategoryId { get; set; }

        public virtual Category? Category { get; set; }
        public virtual ICollection<Feedback> Feedbacks { get; set; }
        public virtual ICollection<ProductDetail> ProductDetails { get; set; }
    }
}
