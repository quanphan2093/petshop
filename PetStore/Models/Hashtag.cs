using System;
using System.Collections.Generic;

namespace PetStore.Models
{
    public partial class Hashtag
    {
        public Hashtag()
        {
            PostHashtags = new HashSet<PostHashtag>();
        }

        public int HashtagId { get; set; }
        public string Tag { get; set; } = null!;

        public virtual ICollection<PostHashtag> PostHashtags { get; set; }
    }
}
