using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagementSystem_HMS_.Dtos.AppDtos.DoctorDtos
{
    public class DoctorViewAppointmentDto
    {
        public Guid AppointmentId { get; set; }
        public string PatientName { get; set; }
        public string PatientGender { get; set; }
        public int PatientAge { get; set; }
        public DateTime AppointmentDate { get; set; }
        public decimal TotalPaid { get; set; }
        public string Status { get; set; }
    }
}
