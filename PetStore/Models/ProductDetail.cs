using System;
using System.Collections.Generic;

namespace PetStore.Models
{
    public partial class ProductDetail
    {
        public ProductDetail()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int ProductDetailId { get; set; }
        public int? ProductId { get; set; }

        public virtual Product? Product { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
