using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagementSystem_HMS_.Dtos.AppDtos.DoctorDtos
{
    public class PrescribedLabTestItemDto
    {
        public Guid LabTestId { get; set; }
        public string TestName { get; set; }
        public decimal Cost { get; set; }
    }
}
