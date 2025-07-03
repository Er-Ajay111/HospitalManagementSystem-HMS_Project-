using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagementSystem_HMS_.Dtos.AppDtos.PatientDtos
{
    public class PendingLabTestListDto
    {
        public string PatientId { get; set; }
        public string PatientName { get; set; }
        public string DoctorName { get; set; }
        public List<PendingLabTestItemDto> Tests { get; set; } = new();
        public decimal TotalAmount { get; set; }
    }
}
