﻿using System;
using System.Collections.Generic;

namespace PetStore.Models
{
    public partial class Product
    {
        public Product()
        {
            Feedbacks = new HashSet<Feedback>();
            OrderDetails = new HashSet<OrderDetail>();
            ProductImages = new HashSet<ProductImage>();
            ShoppingCarts = new HashSet<ShoppingCart>();
        }

        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? Details { get; set; }
        public decimal? Price { get; set; }
        public string? Image { get; set; }
        public double? Discount { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public string? Status { get; set; }
        public int? CategoryId { get; set; }
        public int? UnitInStock { get; set; }
        public int? UnitOrdered { get; set; }
        public int? Size { get; set; }
        public int? ShopId { get; set; }

        public virtual Category? Category { get; set; }
        public virtual Shop? Shop { get; set; }
        public virtual ICollection<Feedback> Feedbacks { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<ProductImage> ProductImages { get; set; }
        public virtual ICollection<ShoppingCart> ShoppingCarts { get; set; }
    }
}
