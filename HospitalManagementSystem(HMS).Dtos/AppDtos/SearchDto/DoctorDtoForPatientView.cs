using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagementSystem_HMS_.Dtos.AppDtos.SearchDto
{
    public class DoctorDtoForPatientView
    {
        public string DoctorId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Specialization { get; set; }
        public List<string> Qualifications { get; set; }
        public List<string> AvailableDays { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string State { get; set; }
        public string City { get; set; }
    }
}
