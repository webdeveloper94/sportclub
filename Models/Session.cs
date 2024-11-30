using System;

namespace SportCenter.Models
{
    public class Session
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string Status { get; set; } // Active, Completed, Cancelled
        public string Notes { get; set; }

        // Navigation property
        public virtual Member Member { get; set; }
    }
}
