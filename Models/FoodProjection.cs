using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LasDeliciasERP.Models
{
    public class FoodProjection
    {
        public int TotalBirds { get; set; }
        public decimal TotalFoodKg { get; set; }
        public decimal DailyConsumptionKg { get; set; }
        public decimal AvailableDays { get; set; }
    }
}