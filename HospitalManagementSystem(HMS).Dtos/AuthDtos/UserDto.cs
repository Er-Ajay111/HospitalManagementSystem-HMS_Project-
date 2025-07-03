using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagementSystem_HMS_.Dtos.AuthDtos
{
    public class UserDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public string AadharNo { get; set; }
        public string Email { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public int Age { get; set; }
        public DateTime DOB { get; set; }
        public string ContactNumber { get; set; }
    }
}
