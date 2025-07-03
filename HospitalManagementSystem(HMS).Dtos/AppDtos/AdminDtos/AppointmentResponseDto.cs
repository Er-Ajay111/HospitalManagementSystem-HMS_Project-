using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HospitalManagementSystem_HMS_.DB.Model.AppModel.AppointmentModel;

namespace HospitalManagementSystem_HMS_.Dtos.AppDtos.AdminDtos
{
    public class AppointmentResponseDto
    {
        public Guid AppointmentId { get; set; }
        public string DoctorId { get; set; } // FK to ApplicationUser.Id (Doctor)
        public string DoctorName { get; set; }
        public string PatientId { get; set; } // FK to ApplicationUser.Id (Patient)
        public string PatientName { get; set; }
        public string PatientGender { get; set; }
        public int PatientAge { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }
        public DateTime BookingDate { get; set; }
    }
}
