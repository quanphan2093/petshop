using System;
using System.Collections.Generic;

namespace PetStore.Models
{
    public partial class ForumType
    {
        public ForumType()
        {
            Forums = new HashSet<Forum>();
        }

        public int TypeId { get; set; }
        public string? TypeName { get; set; }

        public virtual ICollection<Forum> Forums { get; set; }
    }
}
