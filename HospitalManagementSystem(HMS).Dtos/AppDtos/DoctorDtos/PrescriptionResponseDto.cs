using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagementSystem_HMS_.Dtos.AppDtos.DoctorDtos
{
    public class PrescriptionResponseDto
    {
        public Guid PrescriptionId { get; set; }
        public Guid AppointmentId { get; set; }
        public DateTime PrescribeDate { get; set; }
        public string DoctorName { get; set; }
        public string PatientName { get; set; }
        public List<MedicineInfoResponseDto> MedicinesInfoDto { get; set; }
    }
}
