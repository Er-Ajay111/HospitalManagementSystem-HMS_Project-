using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagementSystem_HMS_.Dtos.AppDtos.DoctorDtos
{
    public class LabTestDetailDto
    {
        public Guid LabTestId { get; set; }
        public string TestName { get; set; }
        public string SampleRequired { get; set; }
        public string Preparation { get; set; }
        public string Duration { get; set; }
        public decimal Cost { get; set; }
    }
}
