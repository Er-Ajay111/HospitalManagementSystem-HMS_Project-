using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace HospitalManagementSystem_HMS_.Dtos.AppDtos.LabTechnicianDto
{
    public class LabTestResultUploadDto
    {
        public string Result { get; set; } = string.Empty;
        public IFormFile? ReportFile { get; set; } // optional PDF file
    }
}
