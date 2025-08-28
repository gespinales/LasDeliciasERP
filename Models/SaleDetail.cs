using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LasDeliciasERP.Models
{
    [Serializable]
    public class SaleDetail
    {
        public int Id { get; set; }
        public int SaleId { get; set; }
        public int ProductId { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal SalePrice { get; set; }

        // Otros campos para formularios
        public string DisplayName { get; set; }

        public string ProductName { get; set; }
        public int UnitTypeId { get; set; }

        public string UnitName { get; set; }
        public int EggSizeId { get; set; }
        public string EggSizeName { get; set; }
    }
}