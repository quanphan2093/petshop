using System;
using System.Collections.Generic;

namespace PetStore.Models
{
    public partial class Order
    {
        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int OrderId { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public string? Note { get; set; }
        public int? AddressId { get; set; }
        public int? StatusId { get; set; }

        public virtual Address? Address { get; set; }
        public virtual StatusOrder? Status { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
