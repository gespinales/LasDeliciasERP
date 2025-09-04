using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LasDeliciasERP.Models
{
    public class EggLayingEfficiency
    {
        public int BarnId { get; set; }
        public string BarnName { get; set; }
        public int TotalBirds { get; set; }
        public int TotalEggs { get; set; }
        public decimal EfficiencyPercentage { get; set; } // % de postura
        public decimal ExpectedMin { get; set; } = 90;   // estándar mínimo
        public decimal ExpectedMax { get; set; } = 95;   // estándar máximo
        public DateTime ProductionDate { get; set; }
    }
}