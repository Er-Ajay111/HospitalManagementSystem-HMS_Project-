using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HospitalManagementSystem_HMS_.Dtos.AppDtos.PatientDtos;

namespace HospitalManagementSystem_HMS_.BL.AppRepo.Utility.IService
{
    public interface IPdfService
    {
        byte[] GeneratePrescriptionPdf(PrescriptionPdfDto dto);
    }
}
