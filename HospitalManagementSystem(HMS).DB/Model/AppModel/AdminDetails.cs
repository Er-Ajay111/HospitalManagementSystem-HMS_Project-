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
    public class AdminDetails
    {
        [Key]
        public int AdminId { get; set; }
        public string OwnerName { get; set; }
        public string OwnerContactNo { get; set; }
        public string HospitalLicense { get; set; }
        public string GSTNumber { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser AppUser { get; set; }
    }
}
