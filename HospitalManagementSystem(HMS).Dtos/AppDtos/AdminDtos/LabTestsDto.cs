using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagementSystem_HMS_.Dtos.AppDtos.AdminDtos
{
    public class LabTestsDto
    {
        public string TestName { get; set; }
        public string? Description { get; set; }
        public string SampleRequired { get; set; }  // e.g., Blood, Urine
        public string? Preparation { get; set; }   // e.g., Fasting 12 hrs
        public decimal Cost { get; set; }
        public string? Duration { get; set; }  // e.g., "24 hours"
    }
}
