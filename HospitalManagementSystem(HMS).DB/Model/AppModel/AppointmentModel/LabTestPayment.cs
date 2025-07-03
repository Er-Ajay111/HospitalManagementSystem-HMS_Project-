using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagementSystem_HMS_.DB.Model.AppModel.AppointmentModel
{
    public class LabTestPayment
    {
        [Key]
        public Guid PaymentId { get; set; } = Guid.NewGuid();

        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public bool IsPaid { get; set; } = false;
        public LabPaymentStatus PaymentStatus { get; set; } = LabPaymentStatus.Pending;

        public DateTime PaymentDate { get; set; }

        public ICollection<LabTestPaymentMapping> PaymentMappings { get; set; }
    }
    public enum LabPaymentStatus
    {
        Pending,
        Successful,
        Failed,
    }
}
