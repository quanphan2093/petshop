using System;
using System.Collections.Generic;

namespace PetStore.Models
{
    public partial class Shop
    {
        public Shop()
        {
            Products = new HashSet<Product>();
        }

        public int ShopId { get; set; }
        public string? ShopName { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Website { get; set; }
        public string? FacebookUrl { get; set; }
        public DateTime? CreateAt { get; set; }
        public string? Status { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
