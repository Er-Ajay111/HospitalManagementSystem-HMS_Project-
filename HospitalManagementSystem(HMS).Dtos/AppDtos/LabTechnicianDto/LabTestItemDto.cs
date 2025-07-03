using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagementSystem_HMS_.Dtos.AppDtos.LabTechnicianDto
{
    public class LabTestItemDto
    {
        public Guid LabTestId { get; set; }
        public string TestName { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public DateTime TestDate { get; set; }
        public string? Notes { get; set; }
        public string DoctorName { get; set; }
        public DateTime AppointmentDate { get; set; }
    }
}
