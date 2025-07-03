using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagementSystem_HMS_.Dtos.AppDtos.PatientDtos
{
    public class PendingLabTestItemDto
    {
        public Guid PatientLabTestId { get; set; }
        public Guid LabTestId { get; set; }
        public string TestName { get; set; }
        public string SampleRequired { get; set; }  // e.g., Blood, Urine
        public string? Preparation { get; set; }   // e.g., Fasting 12 hrs
        public DateTime TestDate { get; set; }
        public decimal Cost { get; set; }
    }
}
