using System;

namespace SportCenter.Models
{
    public class Subscription
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public Member Member { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Price { get; set; }
        public SubscriptionType Type { get; set; }
        public bool IsActive { get; set; }
    }

    public enum SubscriptionType
    {
        Daily,
        Hourly
    }
}
