using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagementSystem_HMS_.Dtos.AppDtos.PatientDtos
{
    public class LabTestPaymentRequestDto
    {
        public List<Guid> PatientLabTestIds { get; set; } = new();
        public decimal PaidAmount { get; set; }
    }
}
