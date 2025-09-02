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

        // Factores de conversión a libras individuales
        public const int Quintal = 100;         // 1 quinta = 1 libras
        public const int Arroba = 25;         // 1 arroba = 25 libras
        public const int Libra = 1;         // 1 libra
    }
}