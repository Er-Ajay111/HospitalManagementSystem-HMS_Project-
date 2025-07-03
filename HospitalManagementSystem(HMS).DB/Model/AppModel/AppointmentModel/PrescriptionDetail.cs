using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagementSystem_HMS_.DB.Model.AppModel.AppointmentModel
{
    public class PrescriptionDetail
    {
        public Guid Id { get; set; }
        public Guid PrescriptionId { get; set; }

        public string MedicineName { get; set; }
        public string Dosage { get; set; }
        public string Frequency { get; set; }
        public string Duration { get; set; }
        public string? Instructions { get; set; }  // Optional instructions for the patient

        public Prescription Prescription { get; set; }
    }
}
