using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HospitalManagementSystem_HMS_.Dtos.AuthDtos;

namespace HospitalManagementSystem_HMS_.Dtos.AppDtos
{
    public class DoctorDetailsDto:CommonDetailsDto
    {
        public string Specialization { get; set; }
        public int ExperienceYears { get; set; }
        public decimal ConsultantFee { get; set; }
        public List<string> Qualifications { get; set; }
        public string AlternatePhoneNo { get; set; }
        public string LicenseNumber { get; set; }
        public DateTime LicenseExpiryDate { get; set; }
        public List<string> AvailableDays { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string RoomNumber { get; set; }
    }
}
