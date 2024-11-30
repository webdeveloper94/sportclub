using System;
using System.Collections.Generic;

namespace SportCenter.Models
{
    public class Member
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime RegistrationDate { get; set; }
        public bool IsActive { get; set; }
        public string Address { get; set; }
        public int? TrainerId { get; set; }
        public virtual Trainer Trainer { get; set; }
        public virtual ICollection<ActiveSession> ActiveSessions { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
        public virtual ICollection<Session> Sessions { get; set; }

        public Member()
        {
            Payments = new List<Payment>();
            Sessions = new List<Session>();
        }
    }
}
