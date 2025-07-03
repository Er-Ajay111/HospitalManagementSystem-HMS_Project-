using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HospitalManagementSystem_HMS_.DB.Model.AppModel;
using HospitalManagementSystem_HMS_.DB.Model.AppModel.AppointmentModel;
using Microsoft.AspNetCore.Identity;

namespace HospitalManagementSystem_HMS_.DB.Model.AuthModel
{
    public class ApplicationUser:IdentityUser
    {
        public string Name { get; set; }
        public string Gender { get; set; }
        public string AadharNumber { get; set; }
        public int Age { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }

        public ICollection<Appointment> PatientAppointments { get; set; }
        public ICollection<Appointment> DoctorAppointments { get; set; }
        public PatientDetails PatientDetails { get; set; }
    }
}
