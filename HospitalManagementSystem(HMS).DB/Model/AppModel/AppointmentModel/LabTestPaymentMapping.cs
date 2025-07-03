using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagementSystem_HMS_.DB.Model.AppModel.AppointmentModel
{
    public class LabTestPaymentMapping
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        // 🔗 Foreign Key for Payment
        [ForeignKey("LabTestPayment")]
        public Guid PaymentId { get; set; }
        public LabTestPayment LabTestPayment { get; set; }

        // 🔗 Foreign Key for PatientLabTest
        [ForeignKey("PatientLabTest")]
        public Guid PatientLabTestId { get; set; }
        public PatientLabTests PatientLabTest { get; set; }

    }
}
