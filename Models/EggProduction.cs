using System;
using System.Collections.Generic;

namespace LasDeliciasERP.Models
{
    [Serializable]
    public class EggProduction
    {
        // Campos de la tabla
        public int Id { get; set; }
        public DateTime ProductionDate { get; set; }
        public int BarnId { get; set; }  // llave foránea del galón
        public string Notes { get; set; }

        // Campos adicionales para pintar en pantallas
        public string EggTypeName { get; set; }
        public string BarnName { get; set; }

        // Totales por tamaño, no pertenecen a la tabla, son solo de transformación
        public int QuantityS { get; set; }
        public int QuantityM { get; set; }
        public int QuantityL { get; set; }
        public int QuantityXL { get; set; }

        public int TotalQuantity { get; set; }
        public double TotalWeight { get; set; }
        public double AverageWeight { get; set; }

        // Lista de detalles de esta producción
        public List<EggProductionDetail> Details { get; set; } = new List<EggProductionDetail>();
    }
}
