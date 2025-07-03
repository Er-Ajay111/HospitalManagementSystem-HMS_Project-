using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HospitalManagementSystem_HMS_.DB.Model.AppModel.AppointmentModel;

namespace HospitalManagementSystem_HMS_.Dtos.AppDtos.PatientDtos
{
    public class ShowAppointmentDto
    {
        public string DoctorName { get; set; }
        public DateTime AppointmentDate { get; set; }
        public DateTime BookingDate { get; set; }
        public string Status { get; set; }
        public bool IsPaid { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
