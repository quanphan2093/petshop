using System;
using System.Collections.Generic;

namespace PetStore.Models
{
    public partial class staff
    {
        public staff()
        {
            Messengers = new HashSet<Messenger>();
        }

        public int StaffId { get; set; }
        public string? FullName { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Gender { get; set; }

        public virtual ICollection<Messenger> Messengers { get; set; }
    }
}
