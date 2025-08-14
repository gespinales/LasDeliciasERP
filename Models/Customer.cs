using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LasDeliciasERP.Models
{
    [Serializable]
    public class Customer
    {
        public int Id { get; set; }               
        public string Name { get; set; }          
        public string Phone { get; set; } = "";   // Teléfono opcional
        public string Email { get; set; } = "";   // Correo opcional
        public string Address { get; set; } = ""; // Dirección opcional
        public string Notes { get; set; } = "";   // Notas opcionales
    }
}