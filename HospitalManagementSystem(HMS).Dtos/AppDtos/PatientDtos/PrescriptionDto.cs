using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HospitalManagementSystem_HMS_.Dtos.AppDtos.DoctorDtos;

namespace HospitalManagementSystem_HMS_.Dtos.AppDtos.PatientDtos
{
    public class PrescriptionDto
    {
        public Guid PrescriptionId { get; set; }
        public DateTime PrescribeDate { get; set; }
        public string Note { get; set; }

        public string DoctorName { get; set; }
        public DateTime AppointmentDate { get; set; }

        public List<PrescriptionDetailsDto> PrescriptionDetails { get; set; }
    }
}
