using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LasDeliciasERP.Models
{
    public class InventoryPurchaseDetail
    {
        public int Id { get; set; }
        public int PurchaseId { get; set; }     
        public int ProductId { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        // Campos para formularios
        public string ProductName { get; set; }
        public string UnitAbbreviation { get; set; }
        public decimal SubTotal => Quantity * UnitPrice;

        public int UnitTypeId { get; set; }
        public string UnitName { get; set; }
    }
}