using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagementSystem_HMS_.Dtos.AppDtos.DoctorDtos
{
    public class CreatePrescriptionDto
    {
        public Guid AppointmentId { get; set; }
        public string? Note { get; set; }
        public List<PrescriptionDetailDto> Medicines { get; set; }
    }
}
