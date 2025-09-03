using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LasDeliciasERP.Models
{
    public class FeedingRecord
    {
        public int Id { get; set; }
        public DateTime FeedingDate { get; set; }
        public decimal Quantity { get; set; }
    }
}