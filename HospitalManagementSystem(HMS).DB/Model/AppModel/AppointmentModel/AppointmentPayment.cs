using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagementSystem_HMS_.DB.Model.AppModel.AppointmentModel
{
    public class AppointmentPayment
    {
        [Key]
        public Guid BillingId { get; set; } = Guid.NewGuid();
        public Guid AppointmentId { get; set; }
        public Appointment Appointment { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; } = null;
    }

    public enum PaymentStatus
    {
        Pending,
        Successful,
        Failed,
    }
}
