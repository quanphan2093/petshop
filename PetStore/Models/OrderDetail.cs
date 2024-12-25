using System;
using System.Collections.Generic;

namespace PetStore.Models
{
    public partial class OrderDetail
    {
        public int OrderDetailId { get; set; }
        public int? OrderId { get; set; }
        public int? ProductDetailId { get; set; }
        public int? Quantity { get; set; }

        public virtual Order? Order { get; set; }
        public virtual ProductDetail? ProductDetail { get; set; }
    }
}
