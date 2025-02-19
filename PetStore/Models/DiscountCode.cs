using System;
using System.Collections.Generic;

namespace PetStore.Models
{
    public partial class DiscountCode
    {
        public DiscountCode()
        {
            Orders = new HashSet<Order>();
        }

        public int CodeId { get; set; }
        public string? Code { get; set; }
        public int? DiscountPercent { get; set; }
        public string? Status { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}
