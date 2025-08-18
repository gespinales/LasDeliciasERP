using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Google.Protobuf.WireFormat;

namespace LasDeliciasERP.Models
{
    public class BirdBatch
    {
        public int Id { get; set; }
        public int BarnId { get; set; }             
        public int BirdTypeId { get; set; }         
        public DateTime BatchDate { get; set; }     
        public int Quantity { get; set; }           
        public int EstimatedAgeWeeks { get; set; }  
        public string Notes { get; set; }          

        // Propiedades de navegación opcionales
        public Barn Barn { get; set; }
        public BirdTypes BirdType { get; set; }
    }
}