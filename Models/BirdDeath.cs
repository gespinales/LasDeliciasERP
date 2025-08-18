using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LasDeliciasERP.Models
{
    public class BirdDeath
    {
        public int Id { get; set; }
        public int BarnId { get; set; }
        public int BirdTypeId { get; set; }
        public int Quantity { get; set; }
        public DateTime DeathDate { get; set; }
        public string Reason { get; set; }

        // Campos de ayuda para mostrar nombres en GridView
        public string BarnName { get; set; }
        public string BirdTypeName { get; set; }
    }
}