using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LasDeliciasERP.Utilities
{
    public class UnitConversion
    {
        // Factores de conversión a huevos individuales
        public const int Egg = 1;           // 1 huevo
        public const int Carton = 30;       // 1 cartón = 30 huevos
        public const int Dozen = 12;        // 1 docena = 12 huevos
        public const int Box = 360;         // 1 caja = 12 cartones * 30 huevos
    }
}