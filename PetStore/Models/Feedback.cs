using System;
using System.Collections.Generic;

namespace PetStore.Models
{
    public partial class Feedback
    {
        public int FeedbackId { get; set; }
        public int? AccountId { get; set; }
        public int? ProductId { get; set; }
        public string? Feedback1 { get; set; }
        public DateTime? CreateAt { get; set; }

        public virtual Account? Account { get; set; }
        public virtual Product? Product { get; set; }
    }
}
