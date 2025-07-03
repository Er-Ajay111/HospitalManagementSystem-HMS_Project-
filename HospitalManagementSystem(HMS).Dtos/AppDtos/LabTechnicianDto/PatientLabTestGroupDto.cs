using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagementSystem_HMS_.Dtos.AppDtos.LabTechnicianDto
{
    public class PatientLabTestGroupDto
    {
        public string PatientId { get; set; }
        public string PatientName { get; set; }
        public List<LabTestItemDto> LabTests { get; set; } = new();
    }
}
