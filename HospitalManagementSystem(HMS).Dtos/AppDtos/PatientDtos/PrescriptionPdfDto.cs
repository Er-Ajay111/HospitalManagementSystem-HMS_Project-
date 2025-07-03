using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagementSystem_HMS_.Dtos.AppDtos.PatientDtos
{
    public class PrescriptionPdfDto
    {
        public string DoctorName { get; set; }
        public string PatientName { get; set; }
        public string Diagnosis { get; set; }
        public string Note { get; set; }
        public DateTime Date { get; set; }
        public List<PrescriptionMedicineDto> Medicines { get; set; }
    }
}
