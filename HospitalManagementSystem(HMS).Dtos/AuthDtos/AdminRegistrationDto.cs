using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagementSystem_HMS_.Dtos.AuthDtos
{
    public class AdminRegistrationDto:RegistrationDto
    {
        public string OwnerName { get; set; }
        public string OwnerContactNo { get; set; }
        public string HospitalLicense { get; set; }
        public string GSTNumber { get; set; }
    }
}
