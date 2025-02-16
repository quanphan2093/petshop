using System;
using System.Collections.Generic;

namespace PetStore.Models
{
    public partial class PostHashtag
    {
        public int PostHashtagsId { get; set; }
        public int ForumId { get; set; }
        public int HashtagId { get; set; }

        public virtual Forum Forum { get; set; } = null!;
        public virtual Hashtag Hashtag { get; set; } = null!;
    }
}
