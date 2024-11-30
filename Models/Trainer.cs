using System;
using System.Collections.Generic;

namespace SportCenter.Models
{
    public class Trainer
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public decimal MonthlyFee { get; set; }
        public virtual ICollection<Member> Members { get; set; }
    }
}
