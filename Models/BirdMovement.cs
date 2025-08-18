using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LasDeliciasERP.Models
{
    public class BirdMovement
    {
        public int Id { get; set; }
        public int? BatchId { get; set; }
        public string MovementType { get; set; } // Entrada | Salida
        public int Quantity { get; set; }
        public DateTime MovementDate { get; set; }
        public string Reason { get; set; }
        public int BirdTypeId { get; set; }
        public int BarnId { get; set; }

        // Relaciones (para mostrar en listas)
        public string BirdTypeName { get; set; }
        public string BarnName { get; set; }
    }
}