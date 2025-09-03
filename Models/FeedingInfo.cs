using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LasDeliciasERP.Models
{
    public class FeedingInfo
    {
        public decimal DailyConsumption { get; set; }
        public DateTime? LastFeedingDate { get; set; }
    }
}