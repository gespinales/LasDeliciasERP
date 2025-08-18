using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Google.Protobuf.WireFormat;

namespace LasDeliciasERP.Models
{
    public class BirdTypes
    {
        public int Id { get; set; }
        public string Species { get; set; }   
        public string Breed { get; set; }    
        public string Description { get; set; }  

        // Opcional: navegación inversa a los lotes y movimientos
        // public List<BirdBatch> BirdBatches { get; set; }
        // public List<BirdMovement> BirdMovements { get; set; }
    }
}