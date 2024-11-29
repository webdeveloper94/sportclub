using System;

namespace SportCenter.Models
{
    public class Price
    {
        public int Id { get; set; }
        public decimal DailyPrice { get; set; }
        public decimal HourlyPrice { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
