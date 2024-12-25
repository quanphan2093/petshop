using System;
using System.Collections.Generic;

namespace PetStore.Models
{
    public partial class StateInfor
    {
        public StateInfor()
        {
            Infors = new HashSet<Infor>();
        }

        public int StateId { get; set; }
        public string? StateName { get; set; }

        public virtual ICollection<Infor> Infors { get; set; }
    }
}
