using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LasDeliciasERP.Utilities
{
    public class ConversionFactor
    {
        public int GetConversionFactor(int unitTypeId)
        {
            // Aquí mapeas los IDs reales de tu tabla UnitTypes
            switch (unitTypeId)
            {
                case 1: return UnitConversion.Box;     // Caja
                case 2: return UnitConversion.Carton;  // Cartón
                case 3: return UnitConversion.Egg;     // Unidad

                case 4: return UnitConversion.Quintal;   // Quintal
                case 5: return UnitConversion.Arroba;     // Arroba
                case 6: return UnitConversion.Libra;   // Libra
                default: return 1; // fallback
            }
        }
    }
}