using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LasDeliciasERP.Models
{
    [Serializable]
    public class EggInventory
    {
        // Relación con el tipo de huevo
        public int EggTypeId { get; set; }
        public string EggTypeName { get; set; } 

        // Cantidades por tamaño
        public int QuantityS { get; set; }
        public int QuantityM { get; set; }
        public int QuantityL { get; set; }
        public int QuantityXL { get; set; }

        // Total de huevos
        public int TotalQuantity => QuantityS + QuantityM + QuantityL + QuantityXL;
    }
}