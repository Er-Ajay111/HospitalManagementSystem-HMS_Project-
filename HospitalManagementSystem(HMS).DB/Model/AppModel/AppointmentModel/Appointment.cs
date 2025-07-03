using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HospitalManagementSystem_HMS_.DB.Model.AuthModel;

namespace HospitalManagementSystem_HMS_.DB.Model.AppModel.AppointmentModel
{
    public class Appointment
    {
        public Guid AppointmentId { get; set; }

        public string PatientId { get; set; }   // FK to ApplicationUser.Id (Patient)
        public ApplicationUser Patient { get; set; }  // Navigation property for Patient

        public string DoctorId { get; set; }    // FK to ApplicationUser.Id (Doctor)
        public ApplicationUser Doctor { get; set; }  // Navigation property for Doctor
        [NotMapped]
        public DoctorDetails DoctorDetail { get; set; }  // Navigation property for Doctor
        public DateTime BookingDate { get; set; }
        public DateTime AppointmentDate { get; set; }

        public AppointmentStatus Status { get; set; }      // Enum preferred

        public string Remark { get; set; }

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public bool IsPaid { get; set; } = false;
        public DateTime? PaymentDate { get; set; } = null;
    }

    public enum AppointmentStatus
    {
        Pending,    // Abhi confirm nahi hua, 0
        Confirmed,  // Confirmed ho gaya, 1
        Cancelled,  // Cancel ho gaya, 2
        Completed   // Appointment complete ho chuka, 3
    }
}
