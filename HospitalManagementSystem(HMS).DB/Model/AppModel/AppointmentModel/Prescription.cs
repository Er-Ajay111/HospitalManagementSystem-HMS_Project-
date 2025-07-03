using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagementSystem_HMS_.DB.Model.AppModel.AppointmentModel
{
    public class Prescription
    {
        public Guid PrescriptionId { get; set; }
        public Guid AppointmentId { get; set; }
        public DateTime PrescribeDate { get; set; }
        public string? Note { get; set; }

        public Appointment Appointment { get; set; }   // Navigation
        public ICollection<PrescriptionDetail> PrescriptionDetails { get; set; }
    }
}
