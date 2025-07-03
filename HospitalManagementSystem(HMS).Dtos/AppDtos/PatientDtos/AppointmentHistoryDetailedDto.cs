using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagementSystem_HMS_.Dtos.AppDtos.PatientDtos
{
    public class AppointmentHistoryDetailedDto
    {
        public Guid AppointmentId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string DoctorName { get; set; }
        public string Specialization { get; set; }
        public List<string> Qualifications { get; set; }
        public decimal ConsultationFee { get; set; }

        public string Diagnosis { get; set; }               // Optional: if diagnosis saved
        public string? Notes { get; set; }                  // Prescription note
        public DateTime? PrescriptionDate { get; set; }     // If available

        public List<PrescriptionMedicineDto> Medicines { get; set; }

        public bool IsPaid { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? PaidAmount { get; set; }
    }
}
