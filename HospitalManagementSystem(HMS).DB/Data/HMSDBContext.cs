using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HospitalManagementSystem_HMS_.DB.Model.AppModel;
using HospitalManagementSystem_HMS_.DB.Model.AppModel.AppointmentModel;
using HospitalManagementSystem_HMS_.DB.Model.AuthModel;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem_HMS_.DB.Data
{
    public class HMSDBContext : IdentityDbContext<ApplicationUser>
    {
        public HMSDBContext(DbContextOptions<HMSDBContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Appointment - Patient relation
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Patient)
                .WithMany(u => u.PatientAppointments)   // ApplicationUser me ye collection hona chahiye
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            // Appointment - Doctor relation
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Doctor)
                .WithMany(u => u.DoctorAppointments)    // ApplicationUser me ye collection hona chahiye
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PatientLabTests>()
                .HasOne(p => p.Doctor)
                .WithMany()
                .HasForeignKey(p => p.DoctorId)
                .OnDelete(DeleteBehavior.Restrict); // or NoAction

            modelBuilder.Entity<PatientLabTests>()
                .HasOne(p => p.Patient)
                .WithMany()
                .HasForeignKey(p => p.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<ChemistDetails> Chemist_tbl { get; set; }
        public DbSet<LabTechnicianDetails> LabTechnician_tbl { get; set; }
        public DbSet<NurseDetails> Nurse_tbl { get; set; }
        public DbSet<DoctorDetails> Doctor_tbl { get; set; }
        public DbSet<PatientDetails> Patient_tbl { get; set; }
        public DbSet<AdminDetails> Admin_tbl { get; set; }
        public DbSet<Appointment> Appointment_tbl { get; set; }
        public DbSet<Prescription> Prescription_tbl { get; set; }
        public DbSet<PrescriptionDetail> PrescriptionDetails_tbl { get; set; }
        public DbSet<LabTests> LabTests_tbl { get; set; }
        public DbSet<PatientLabTests> PatientLabTests_tbl { get; set; }
        public DbSet<AppointmentPayment> AppointmentPayment_tbl { get; set; }
        public DbSet<LabTestPayment> LabTestPayment_tbl { get; set; }
        public DbSet<LabTestPaymentMapping> LabTestPaymentMapping_tbl { get; set; }
    }
}
