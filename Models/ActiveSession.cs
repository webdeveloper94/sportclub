using System;

namespace SportCenter.Models
{
    public class ActiveSession
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public Member Member { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public decimal? TotalAmount { get; set; }
        public string SessionType { get; set; }
    }
}
