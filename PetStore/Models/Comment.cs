using System;
using System.Collections.Generic;

namespace PetStore.Models
{
    public partial class Comment
    {
        public int CommentId { get; set; }
        public int? ForumId { get; set; }
        public int? ParentCommentId { get; set; }
        public string? Content { get; set; }
        public DateTime? CreateAt { get; set; }
        public int? AccountId { get; set; }

        public virtual Account? Account { get; set; }
        public virtual Forum? Forum { get; set; }
    }
}
