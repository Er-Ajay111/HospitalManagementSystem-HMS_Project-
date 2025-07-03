using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagementSystem_HMS_.Dtos.AppDtos.DoctorDtos
{
    public class PrescribeLabTestResponseDto
    {
        public string PatientId { get; set; }
        public string PatientName { get; set; }

        public string OrderedById { get; set; }
        public string DoctorName { get; set; }
        public Guid AppointmentId { get; set; } // FK to Appointment

        public DateTime TestDate { get; set; }

        public List<PrescribedLabTestItemDto> LabTests { get; set; }
    }
}
