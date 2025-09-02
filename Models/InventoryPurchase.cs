using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LasDeliciasERP.Models
{
    public class InventoryPurchase
    {
        public int Id { get; set; }
        public int SupplierId { get; set; }
        public DateTime PurchaseDate { get; set; }
        public string SupplierName { get; set; }
        public string InvoiceNumber { get; set; }
        public decimal TotalAmount { get; set; }
        public string Notes { get; set; }

        // Detalles de la compra
        public List<InventoryPurchaseDetail> Details { get; set; } = new List<InventoryPurchaseDetail>();
    }
}