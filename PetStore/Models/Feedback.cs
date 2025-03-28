﻿using System;
using System.Collections.Generic;

namespace PetStore.Models
{
    public partial class Feedback
    {
        public int FeedbackId { get; set; }
        public int? AccountId { get; set; }
        public int? ProductId { get; set; }
        public string? Feedback1 { get; set; }
        public string? Image { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int? Love { get; set; }
        public int? Like { get; set; }
        public int? Dislike { get; set; }

        public virtual Account? Account { get; set; }
        public virtual Product? Product { get; set; }
    }
}
