using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LasDeliciasERP.Utilities
{
    public class Maps
    {
        public Dictionary<int, int> GetEggProductMap()
        {
            return new Dictionary<int, int>
            {
                // Huevos blancos S para todas las unidades de medida se juntan a Huevo blanco S unidad
                { 1, 9 }, { 5, 9 }, { 9, 9 },

                // Huevos blancos M
                { 2, 10 }, { 6, 10 }, { 10, 10 },

                // Huevos blancos L
                { 3, 11 }, { 7, 11 }, { 11, 11 },

                // Huevos blancos XL
                { 4, 12 }, { 8, 12 }, { 12, 12 },

                // Huevos marrones S
                { 13, 21 }, { 17, 21 }, { 21, 21 },

                // Huevos marrones M
                { 14, 22 }, { 18, 22 }, { 22, 22 },

                // Huevos marrones L
                { 15, 23 }, { 19, 23 }, { 23, 23 },

                // Huevos marrones XL
                { 16, 24 }, { 20, 24 }, { 24, 24 }
            };
        }

        public Dictionary<int, int> GetProductMap()
        {
            return new Dictionary<int, int>
            {
                // Huevos blancos S para todas las unidades de medida se juntan a Huevo blanco S unidad
                { 25, 26 }, { 26, 26 }
            };
        }
    }
}