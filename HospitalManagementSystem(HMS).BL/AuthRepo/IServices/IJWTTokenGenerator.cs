using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HospitalManagementSystem_HMS_.DB.Model.AuthModel;

namespace HospitalManagementSystem_HMS_.BL.AuthRepo.IServices
{
    public interface IJWTTokenGenerator
    {
        string GenerateJWTToken(ApplicationUser user, IList<string> roles);
    }
}
