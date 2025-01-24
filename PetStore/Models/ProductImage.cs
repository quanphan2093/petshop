using System;
using System.Collections.Generic;

namespace PetStore.Models
{
    public partial class ProductImage
    {
        public int ImgId { get; set; }
        public string? ImgUrl { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public string? Status { get; set; }
        public int? ProductId { get; set; }

        public virtual Product? Product { get; set; }
    }
}
