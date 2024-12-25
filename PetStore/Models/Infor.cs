using System;
using System.Collections.Generic;

namespace PetStore.Models
{
    public partial class Infor
    {
        public int InforId { get; set; }
        public string? Fullname { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? Gender { get; set; }
        public string? Image { get; set; }
        public int? AccountId { get; set; }
        public int? StateId { get; set; }

        public virtual Account? Account { get; set; }
        public virtual StateInfor? State { get; set; }
    }
}
