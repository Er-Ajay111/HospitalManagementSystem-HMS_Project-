using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HospitalManagementSystem_HMS_.DB.Model.AppModel.AppointmentModel;

namespace HospitalManagementSystem_HMS_.Dtos.AppDtos.DoctorDtos
{
    public class PrescribeLabTestDto
    {
        public Guid AppointmentId { get; set; }
        public DateTime TestDate { get; set; }
        public string? Notes { get; set; }          // Doctor can write remarks while prescribing
        public TestPriority Priority { get; set; } = TestPriority.Normal; // Default priority
        public List<Guid> LabTestIds { get; set; }
    }
}
