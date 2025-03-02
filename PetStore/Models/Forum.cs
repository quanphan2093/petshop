using System;
using System.Collections.Generic;

namespace PetStore.Models
{
    public partial class Forum
    {
        public Forum()
        {
            CommentsNavigation = new HashSet<Comment>();
            PostHashtags = new HashSet<PostHashtag>();
        }

        public int ForumId { get; set; }
        public string? Content { get; set; }
        public int? CommentId { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public string? Status { get; set; }
        public int? AccountId { get; set; }
        public string? Image { get; set; }
        public int? Age { get; set; }
        public bool? Gene { get; set; }
        public int? Views { get; set; }
        public int? Likes { get; set; }
        public int? Comments { get; set; }
        public string? Title { get; set; }
        public int? TypeId { get; set; }
        public bool? IsPinned { get; set; }

        public virtual Account? Account { get; set; }
        public virtual ForumType? Type { get; set; }
        public virtual ICollection<Comment> CommentsNavigation { get; set; }
        public virtual ICollection<PostHashtag> PostHashtags { get; set; }
    }
}
