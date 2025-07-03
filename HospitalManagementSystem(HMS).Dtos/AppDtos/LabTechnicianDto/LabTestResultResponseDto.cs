using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagementSystem_HMS_.Dtos.AppDtos.LabTechnicianDto
{
    public class LabTestResultResponseDto
    {
        public Guid LabTestId { get; set; }
        public string TestName { get; set; }
        public string PatientName { get; set; }
        public string DoctorName { get; set; }
        public string Status { get; set; }
        public string? ReportFilePath { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
