using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LasDeliciasERP.Models
{
    public class Barn
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public string Notes { get; set; }
    }
}