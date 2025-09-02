using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LasDeliciasERP.Models
{
    public class VaccinationSchedule
    {
        public int Id { get; set; }
        public int BarnId { get; set; }
        public int VaccineId { get; set; }
        public DateTime ScheduledDate { get; set; }
        public string Status { get; set; }   // Pending / Applied / Overdue
        public string Notes { get; set; }

        // Campos adicionales para mostrar en la vista (JOINs con Barn y Vaccine)
        public string BarnName { get; set; }
        public string VaccineName { get; set; }
    }
}