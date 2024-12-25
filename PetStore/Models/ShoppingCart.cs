using System;
using System.Collections.Generic;

namespace PetStore.Models
{
    public partial class ShoppingCart
    {
        public int CartId { get; set; }
        public int? AccountId { get; set; }
        public int? Quantity { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int? ProductId { get; set; }

        public virtual Account? Account { get; set; }
        public virtual Product? Product { get; set; }
    }
}
