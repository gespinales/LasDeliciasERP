using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LasDeliciasERP.Models
{
    public class Vaccine
    {
        public int Id { get; set; }                  
        public string Name { get; set; }             
        public string Description { get; set; }     
        public string RecommendedDose { get; set; }  
        public int IntervalDays { get; set; }       
        public string AdministrationRoute { get; set; }       
    }
}