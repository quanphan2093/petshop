using System;
using System.Collections.Generic;

namespace PetStore.Models
{
    public partial class Address
    {
        public Address()
        {
            Orders = new HashSet<Order>();
        }

        public int AddressId { get; set; }
        public string? FullNameCustomer { get; set; }
        public string? Phone { get; set; }
        public string? Address1 { get; set; }
        public string? Email { get; set; }
        public string? Gender { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}
