using System;
using System.Collections.Generic;

namespace PetStore.Models
{
    public partial class OrderDetail
    {
        public int OrderDetailId { get; set; }
        public decimal? Total { get; set; }
        public DateTime? CreateAt { get; set; }
        public decimal? UnitPrice { get; set; }
        public int? OrderId { get; set; }
        public int? ProductId { get; set; }
        public int? Quantity { get; set; }

        public virtual Order? Order { get; set; }
        public virtual Product? Product { get; set; }
    }
}
