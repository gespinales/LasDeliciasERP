using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LasDeliciasERP.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? EggTypeId { get; set; }
        public int UnitTypeId { get; set; }
        public string Notes { get; set; }
    }
}