using System;

namespace LasDeliciasERP.Models
{
    [Serializable]
    public class EggProductionDetail
    {
        public int Id { get; set; }
        public int EggProductionId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }   // Nombre y tamaño del huevo
        public int Quantity { get; set; }
    }
}
