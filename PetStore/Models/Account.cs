using System;
using System.Collections.Generic;

namespace PetStore.Models
{
    public partial class Account
    {
        public Account()
        {
            Comments = new HashSet<Comment>();
            Feedbacks = new HashSet<Feedback>();
            Forums = new HashSet<Forum>();
            Messengers = new HashSet<Messenger>();
        }

        public int AccountId { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int? RoleId { get; set; }

        public virtual Role? Role { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Feedback> Feedbacks { get; set; }
        public virtual ICollection<Forum> Forums { get; set; }
        public virtual ICollection<Messenger> Messengers { get; set; }
    }
}
