using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LasDeliciasERP.Models
{
    public class EggProduction
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        
        // Cantidad de huevos por categoría de tamaño
        public int QuantityS { get; set; }
        public int QuantityM { get; set; }
        public int QuantityL { get; set; }
        public int QuantityXL { get; set; }

        // Peso promedio aproximado por categoría (puedes ajustar según estándares locales)
        private readonly Dictionary<EggSize, double> AverageWeightPerSize = new Dictionary<EggSize, double>
        {
            { EggSize.S, 50 },   // gramos
            { EggSize.M, 58 },
            { EggSize.L, 68 },
            { EggSize.XL, 75 }
        };

        // Total de huevos
        public int TotalQuantity => QuantityS + QuantityM + QuantityL + QuantityXL;

        // Total de peso aproximado
        public double TotalWeight => QuantityS * AverageWeightPerSize[EggSize.S]
                                  + QuantityM * AverageWeightPerSize[EggSize.M]
                                  + QuantityL * AverageWeightPerSize[EggSize.L]
                                  + QuantityXL * AverageWeightPerSize[EggSize.XL];

        // Peso promedio aproximado por huevo
        public double AverageWeight => TotalQuantity > 0 ? TotalWeight / TotalQuantity : 0;

        public string Notes { get; set; }


        // Vendrán de la tabla de tipos de huevos
        public int EggTypeId { get; set; }      
        public string EggTypeName { get; set; } 
    }
}