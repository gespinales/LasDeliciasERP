using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LasDeliciasERP.Models
{
    public class Inventory
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Quantity { get; set; }
        public DateTime LastUpdated { get; set; }

        // Campo para formularios
        public string UnitName { get; set; }
    }
}