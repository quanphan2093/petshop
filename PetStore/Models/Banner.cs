using System;
using System.Collections.Generic;

namespace PetStore.Models
{
    public partial class Banner
    {
        public int BannerId { get; set; }
        public string? BannerImage { get; set; }
        public string? BannerUrl { get; set; }
        public int? ClickCount { get; set; }
        public DateTime? CreateAt { get; set; }
        public string? Status { get; set; }
    }
}
