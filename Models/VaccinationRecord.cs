using System;

namespace LasDeliciasERP.Models
{
    public class VaccinationRecord
    {
        public int Id { get; set; }
        public int ScheduleId { get; set; }
        public DateTime AppliedDate { get; set; }
        public int QuantityApplied { get; set; }
        public string AppliedBy { get; set; }
        public string Notes { get; set; }

        // Campos que nos servirán para formularios
        public string VaccineName { get; set; }
        public string BarnName { get; set; }
    }
}
