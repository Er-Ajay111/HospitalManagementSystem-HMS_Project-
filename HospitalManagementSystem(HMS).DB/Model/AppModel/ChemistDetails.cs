using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HospitalManagementSystem_HMS_.DB.Model.AuthModel;

namespace HospitalManagementSystem_HMS_.DB.Model.AppModel
{
    public class ChemistDetails
    {
        [Key]
        public int ChemistId { get; set; }
        public string Qualification { get; set; }
        public string LicenseNumber { get; set; }
        public string LicenseIssuedBy { get; set; }
        public DateTime LicenseExpiryDate { get; set; }
        public int ExperienceInYears { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser AppUser { get; set; }
    }
}
