using System;
using System.Collections.Generic;

namespace PetStore.Models
{
    public partial class Forum
    {
        public Forum()
        {
            Comments = new HashSet<Comment>();
        }

        public int ForumId { get; set; }
        public string? Content { get; set; }
        public DateTime? CreateAt { get; set; }
        public int? AccountId { get; set; }

        public virtual Account? Account { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
    }
}
