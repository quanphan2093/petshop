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
        public string? NoteBySale { get; set; }
        public int? SaleId { get; set; }
        public int? AccountId { get; set; }
        public int? AddressId { get; set; }
        public int? StatusId { get; set; }
        public int? PaymentMethodId { get; set; }
        public int? DiscountId { get; set; }

        public virtual Account? Account { get; set; }
        public virtual Address? Address { get; set; }
        public virtual DiscountCode? Discount { get; set; }
        public virtual PaymentMethod? PaymentMethod { get; set; }
        public virtual StatusOrder? Status { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
