using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LasDeliciasERP.Models
{
    public class Sale
    {
        public int Id { get; set; }
        public DateTime SaleDate { get; set; }
        public int CustomerId { get; set; }
        public string Notes { get; set; }

        // Otros campos para formularios
        public string CustomerName { get; set; }
        public List<SaleDetail> Details { get; set; } = new List<SaleDetail>();
        public decimal TotalQuantity { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalSaleAmount { get; set; }

    }
}