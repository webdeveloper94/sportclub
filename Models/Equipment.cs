using System;

namespace SportCenter.Models
{
    public class Equipment
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime PurchaseDate { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string Status { get; set; } // Active, Under Repair, Retired
    }
}
