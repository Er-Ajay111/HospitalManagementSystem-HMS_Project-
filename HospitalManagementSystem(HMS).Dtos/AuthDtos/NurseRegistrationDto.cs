using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagementSystem_HMS_.Dtos.AuthDtos
{
    public class NurseRegistrationDto:RegistrationDto
    {
        public List<string> Qualifications { get; set; }
        public string RegistrationCouncil { get; set; }
        public string CouncilShortCode { get; set; }
        public int ExperienceInYears { get; set; }
        public List<string> Departments { get; set; }
    }
}
