using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagementSystem_HMS_.Dtos.AuthDtos
{
    public class LabTechnicianRegistrationDto:RegistrationDto
    {
        public List<string> Qualifications { get; set; }
        public string LicenseNumber { get; set; }
        public string LicenseIssuedBy { get; set; }
        public DateTime LicenseExpiryDate { get; set; }
        public int ExperienceInYears { get; set; }
        public List<string> Specializations { get; set; }
        public DateTime JoiningDate { get; set; }
    }
}
