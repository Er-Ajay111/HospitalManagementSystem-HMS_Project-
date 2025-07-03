using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HospitalManagementSystem_HMS_.DB.Model.AuthModel;

namespace HospitalManagementSystem_HMS_.DB.Model.AppModel.AppointmentModel
{
    public class PatientLabTests
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public string PatientId { get; set; }              // FK to ApplicationUser
        public ApplicationUser Patient { get; set; }
      
        public Guid LabTestId { get; set; }                // FK to LabTest
        public LabTests LabTest { get; set; }

        [ForeignKey("DoctorId")]
        public string DoctorId { get; set; }           // FK to Doctor (ApplicationUser)
        public ApplicationUser? Doctor { get; set; }

        public Guid AppointmentId { get; set; }                // 🔴 Add this
        public Appointment Appointment { get; set; }           // 🔴 Navigation property

        public DateTime TestDate { get; set; }
        public LabTestStatus Status { get; set; } = LabTestStatus.Requested;
        public string? Notes { get; set; }          // Doctor can write remarks while prescribing
        public string? Result { get; set; }         // Lab Technician uploads the result
        public string? ReportFilePath { get; set; } // If lab technician uploads PDF/scan
        public TestPriority Priority { get; set; } = TestPriority.Normal; // Default priority
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; } = null;
    }

    public enum LabTestStatus
    {
        Requested,   // Doctor prescribed
        Paid,        // Payment done, ready to test
        InProgress,
        Completed,
        Cancelled
    }
    public enum TestPriority
    {
        Low,
        Normal,
        Urgent,
        Critical
    }
}

