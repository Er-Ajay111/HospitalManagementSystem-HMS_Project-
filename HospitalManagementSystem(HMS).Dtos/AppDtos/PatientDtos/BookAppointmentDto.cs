using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagementSystem_HMS_.Dtos.AppDtos.PatientDtos
{
    public class BookAppointmentDto
    {
        public string DoctorId { get; set; }     
        public DateTime AppointmentDate { get; set; }
        public string Remark { get; set; }
    }
}
