using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagementSystem_HMS_.Dtos.AuthDtos
{
    public class PatientRegistrationDto:RegistrationDto
    {
        public string BloodGroup { get; set; }
        public string ChronicDiseases { get; set; }
        public string MedicalHistory { get; set; }
        public DateTime RegistrationDate { get; set; } = DateTime.Now;
    }
}
