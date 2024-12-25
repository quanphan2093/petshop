using System;
using System.Collections.Generic;

namespace PetStore.Models
{
    public partial class Messenger
    {
        public int MessengerId { get; set; }
        public int? AccountId { get; set; }
        public int? StaffId { get; set; }
        public string? MessageText { get; set; }
        public DateTime? SentTime { get; set; }

        public virtual Account? Account { get; set; }
    }
}
