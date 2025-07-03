using System.Web;
using HospitalManagementSystem_HMS_.BL.AuthRepo.IServices;
using HospitalManagementSystem_HMS_.DB.Data;
using HospitalManagementSystem_HMS_.DB.Model.AppModel;
using HospitalManagementSystem_HMS_.DB.Model.AuthModel;
using HospitalManagementSystem_HMS_.Dtos.AuthDtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem_HMS_.BL.AuthRepo.Implementation
{
    public class AuthService : IAuthService
    {
        //private readonly HMSDBContext _authDB;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJWTTokenGenerator _jwtTokenGenerator;
        private readonly HMSDBContext _hmsDBContext;
        private readonly IEmailService _emailService;

        public AuthService(HMSDBContext authDB, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IJWTTokenGenerator jwtTokenGenerator, HMSDBContext hmsDBContext, IEmailService emailService)
        {
            //_authDB = authDB;
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtTokenGenerator = jwtTokenGenerator;
            _hmsDBContext = hmsDBContext;
            _emailService = emailService;
        }

        public async Task<string> AdminRegistration(AdminRegistrationDto adminRegistrationDto)
        {
            try
            {
                ApplicationUser user = new ApplicationUser()
                {
                    Email = adminRegistrationDto.Email,
                    UserName = adminRegistrationDto.Email,
                    Gender = adminRegistrationDto.Gender,
                    PhoneNumber = adminRegistrationDto.PhoneNumber,
                    Name = adminRegistrationDto.Name,
                    PostalCode = adminRegistrationDto.PostalCode,
                    AadharNumber = adminRegistrationDto.AadharNumber,
                    Age = adminRegistrationDto.Age,
                    DateOfBirth = adminRegistrationDto.DateOfBirth,
                    State = adminRegistrationDto.State,
                    City = adminRegistrationDto.City,
                };
                var result = await RegisterAsync(user, adminRegistrationDto.Password, adminRegistrationDto.Roles);
                if (result)
                {
                    var newUser = new AdminDetails
                    {
                        UserId = user.Id,
                        OwnerName = adminRegistrationDto.OwnerName,
                        OwnerContactNo = adminRegistrationDto.OwnerContactNo,
                        HospitalLicense = adminRegistrationDto.HospitalLicense,
                        GSTNumber = adminRegistrationDto.GSTNumber
                    };
                    await _hmsDBContext.Admin_tbl.AddAsync(newUser);
                    await _hmsDBContext.SaveChangesAsync();
                    return "Admin Registration successful";
                }
                else
                {
                    throw new Exception("Admin Registration failed");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Exception: {ex.Message}");
            }
        }

        public async Task<bool> AssignRolesToUser(string EmailId, List<string> roles)
        {
            try
            {
                var registeredUser = await _userManager.FindByEmailAsync(EmailId);
                if (registeredUser == null)
                {
                    throw new ArgumentException($"User not found with email : {EmailId}");
                }
                foreach (var role in roles)
                {
                    if (!await _roleManager.RoleExistsAsync(role))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(role));
                    }
                    await _userManager.AddToRoleAsync(registeredUser, role);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<string> DoctorRegistration(DoctorRegistrationDto doctorRegistrationDto)
        {
            try
            {
                ApplicationUser user = new ApplicationUser()
                {
                    Email = doctorRegistrationDto.Email,
                    UserName = doctorRegistrationDto.Email,
                    Gender = doctorRegistrationDto.Gender,
                    PhoneNumber = doctorRegistrationDto.PhoneNumber,
                    Name = $"Dr. {doctorRegistrationDto.Name}",
                    PostalCode = doctorRegistrationDto.PostalCode,
                    AadharNumber = doctorRegistrationDto.AadharNumber,
                    Age = doctorRegistrationDto.Age,
                    DateOfBirth = doctorRegistrationDto.DateOfBirth,
                    State = doctorRegistrationDto.State,
                    City = doctorRegistrationDto.City,
                };
                var result = await RegisterAsync(user, doctorRegistrationDto.Password, doctorRegistrationDto.Roles);
                if (result)
                {
                    var newUser = new DoctorDetails
                    {
                        UserId = user.Id,
                        Specialization = doctorRegistrationDto.Specialization,
                        Qualifications = string.Join(",", doctorRegistrationDto.Qualifications),
                        AlternatePhoneNo = doctorRegistrationDto.AlternatePhoneNo,
                        ExperienceYears = doctorRegistrationDto.ExperienceYears,
                        ConsultantFee = doctorRegistrationDto.ConsultantFee,
                        LicenseNumber = GenerateDoctorLicenseNumber(),
                        LicenseExpiryDate = doctorRegistrationDto.LicenseExpiryDate,
                        RoomNumber = doctorRegistrationDto.RoomNumber,
                        AvailableDays = string.Join(",", doctorRegistrationDto.AvailableDays),
                        StartTime = doctorRegistrationDto.StartTime,
                        EndTime = doctorRegistrationDto.EndTime,
                        IsActive = true
                    };
                    await _hmsDBContext.Doctor_tbl.AddAsync(newUser);
                    await _hmsDBContext.SaveChangesAsync();
                    return "Doctor Registration successful";
                }
                else
                {
                    throw new Exception("Doctor Registration failed");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Exception: {ex.Message}");
            }
        }
        private string GenerateDoctorLicenseNumber()
        {
            // Example format: DOC-20250531-83729
            string prefix = "DOC";
            string datePart = DateTime.UtcNow.ToString("yyyyMMdd"); // 20250531
            string randomPart = new Random().Next(10000, 99999).ToString(); // 83729
            return $"{prefix}-{datePart}-{randomPart}";
        }

        public async Task<LoginResponseDto> LoginAsync(LoginDto loginDto)
        {
            try
            {
                if (loginDto == null)
                {
                    throw new Exception("User data is null");
                }
                var user = _hmsDBContext.ApplicationUsers.FirstOrDefault(x => x.Email == loginDto.Email);
                if (user == null)
                {
                    throw new Exception($"User with email {loginDto.Email} not found");
                }
                var isValid = await _userManager.CheckPasswordAsync(user, loginDto.Password);
                if (!isValid)
                {
                    throw new Exception("Invalid password");
                }
                UserDto userDto = new UserDto()
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Gender = user.Gender,
                    Age = user.Age,
                    AadharNo = user.AadharNumber,
                    ContactNumber = user.PhoneNumber,
                    State = user.State,
                    City = user.City,
                    DOB = user.DateOfBirth
                };
                var roles = await _userManager.GetRolesAsync(user);
                var loginResponse = new LoginResponseDto()
                {
                    User = userDto,
                    Token = _jwtTokenGenerator.GenerateJWTToken(user, roles),
                };
                return loginResponse;
            }
            catch (Exception ex)
            {
                throw new Exception($"Exception: {ex.Message}");
            }
        }

        private async Task<bool> RegisterAsync(ApplicationUser appUser, string password, List<string> roles)
        {
            try
            {
                var existingUser = await _hmsDBContext.ApplicationUsers.FirstOrDefaultAsync(x => x.Email == appUser.Email);
                if (existingUser != null)
                {
                    throw new ArgumentException($"User already exist with this email {appUser.Email}");
                }
                var result = await _userManager.CreateAsync(appUser, password);
                if (!result.Succeeded)
                {
                    throw new Exception("registration failed");
                }
                //assign role to user 
                await AssignRolesToUser(appUser.Email, roles);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<string> PatientRegistration(PatientRegistrationDto patientRegistrationDto)
        {
            try
            {
                ApplicationUser user = new ApplicationUser()
                {
                    Email = patientRegistrationDto.Email,
                    UserName = patientRegistrationDto.Email,
                    Gender = patientRegistrationDto.Gender,
                    PhoneNumber = patientRegistrationDto.PhoneNumber,
                    Name = patientRegistrationDto.Name,
                    PostalCode = patientRegistrationDto.PostalCode,
                    AadharNumber = patientRegistrationDto.AadharNumber,
                    Age = patientRegistrationDto.Age,
                    DateOfBirth = patientRegistrationDto.DateOfBirth,
                    State = patientRegistrationDto.State,
                    City = patientRegistrationDto.City,
                };
                var result = await RegisterAsync(user, patientRegistrationDto.Password, patientRegistrationDto.Roles);
                if (result)
                {
                    var newPatient = new PatientDetails
                    {
                        UserId = user.Id,
                        BloodGroup = patientRegistrationDto.BloodGroup,
                        MedicalHistory = patientRegistrationDto.MedicalHistory,
                        ChronicDiseases = patientRegistrationDto.ChronicDiseases
                    };
                    newPatient.RegistrationDate = DateTime.Now;
                    await _hmsDBContext.Patient_tbl.AddAsync(newPatient);
                    await _hmsDBContext.SaveChangesAsync();
                    return "Patient Registration successful";
                }
                else
                {
                    return "Patient Registration failed";
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<string> NurseRegistration(NurseRegistrationDto nurseRegistrationDto)
        {
            try
            {
                ApplicationUser user = new ApplicationUser()
                {
                    Email = nurseRegistrationDto.Email,
                    UserName = nurseRegistrationDto.Email,
                    Gender = nurseRegistrationDto.Gender,
                    PhoneNumber = nurseRegistrationDto.PhoneNumber,
                    Name = nurseRegistrationDto.Name,
                    PostalCode = nurseRegistrationDto.PostalCode,
                    AadharNumber = nurseRegistrationDto.AadharNumber,
                    Age = nurseRegistrationDto.Age,
                    DateOfBirth = nurseRegistrationDto.DateOfBirth,
                    State = nurseRegistrationDto.State,
                    City = nurseRegistrationDto.City,
                };
                var result = await RegisterAsync(user, nurseRegistrationDto.Password, nurseRegistrationDto.Roles);
                if (result)
                {
                    var newUser = new NurseDetails
                    {
                        UserId = user.Id,
                        Qualification = string.Join(",", nurseRegistrationDto.Qualifications),
                        Departments = string.Join(",", nurseRegistrationDto.Departments),
                        ExperienceInYears = nurseRegistrationDto.ExperienceInYears,
                        RegistrationCouncil = nurseRegistrationDto.RegistrationCouncil,
                        CouncilRegistrationNumber = GenerateCouncilRegistrationNumber(nurseRegistrationDto.CouncilShortCode)
                    };
                    await _hmsDBContext.Nurse_tbl.AddAsync(newUser);
                    await _hmsDBContext.SaveChangesAsync();
                    return "Nurse Registration successful";
                }
                else
                {
                    throw new Exception("Nurse Registration failed");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Exception: {ex.Message}");
            }
        }
        private string GenerateCouncilRegistrationNumber(string councilShortCode)
        {
            // Ensure councilShortCode is not null or empty
            if (string.IsNullOrWhiteSpace(councilShortCode))
                throw new ArgumentException("Council short code is required");

            // Current year
            int year = DateTime.Now.Year;

            // Random 5-digit number
            Random rnd = new Random();
            int randomNumber = rnd.Next(10000, 99999);

            // Format: COUNCIL/YEAR/RANDOMNUMBER
            return $"{councilShortCode.ToUpper()}/{year}/{randomNumber}";
        }

        public async Task<string> ChemistRegistration(ChemistRegistrationDto chemistRegistrationDto)
        {
            try
            {
                ApplicationUser user = new ApplicationUser()
                {
                    Email = chemistRegistrationDto.Email,
                    UserName = chemistRegistrationDto.Email,
                    Gender = chemistRegistrationDto.Gender,
                    PhoneNumber = chemistRegistrationDto.PhoneNumber,
                    Name = chemistRegistrationDto.Name,
                    PostalCode = chemistRegistrationDto.PostalCode,
                    AadharNumber = chemistRegistrationDto.AadharNumber,
                    Age = chemistRegistrationDto.Age,
                    DateOfBirth = chemistRegistrationDto.DateOfBirth,
                    State = chemistRegistrationDto.State,
                    City = chemistRegistrationDto.City,
                };
                var result = await RegisterAsync(user, chemistRegistrationDto.Password, chemistRegistrationDto.Roles);
                if (result)
                {
                    var newChemist = new ChemistDetails
                    {
                        UserId = user.Id,
                        Qualification = string.Join(",", chemistRegistrationDto.Qualifications),
                        LicenseNumber = ChemistLicenseNumber(),
                        LicenseIssuedBy = chemistRegistrationDto.LicenseIssuedBy,
                        LicenseExpiryDate = chemistRegistrationDto.LicenseExpiryDate,
                        ExperienceInYears = chemistRegistrationDto.ExperienceInYears
                    };
                    await _hmsDBContext.Chemist_tbl.AddAsync(newChemist);
                    await _hmsDBContext.SaveChangesAsync();
                    return "Chemist Registration successful";
                }
                else
                {
                    return "Chemist Registration successful";
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Exception: {ex.Message}");
            }
        }
        private string ChemistLicenseNumber()
        {
            // Current year
            int year = DateTime.Now.Year;

            // Random 5-digit number
            Random rnd = new Random();
            int randomNumber = rnd.Next(10000, 99999);

            // Format: CHEM/YEAR/RANDOMNUMBER
            return $"CHEM/{year}/{randomNumber}";
        }

        public async Task<string> LabTechnicianRegistration(LabTechnicianRegistrationDto labTechnicianRegistrationDto)
        {
            try
            {
                ApplicationUser user = new ApplicationUser()
                {
                    Email = labTechnicianRegistrationDto.Email,
                    UserName = labTechnicianRegistrationDto.Email,
                    Gender = labTechnicianRegistrationDto.Gender,
                    PhoneNumber = labTechnicianRegistrationDto.PhoneNumber,
                    Name = labTechnicianRegistrationDto.Name,
                    PostalCode = labTechnicianRegistrationDto.PostalCode,
                    AadharNumber = labTechnicianRegistrationDto.AadharNumber,
                    Age = labTechnicianRegistrationDto.Age,
                    DateOfBirth = labTechnicianRegistrationDto.DateOfBirth,
                    State = labTechnicianRegistrationDto.State,
                    City = labTechnicianRegistrationDto.City,
                };
                var result = await RegisterAsync(user, labTechnicianRegistrationDto.Password, labTechnicianRegistrationDto.Roles);
                if(result)
                {
                    var newUser = new LabTechnicianDetails
                    {
                        UserId = user.Id,
                        Qualification = string.Join(",", labTechnicianRegistrationDto.Qualifications),
                        ExperienceInYears = labTechnicianRegistrationDto.ExperienceInYears,
                        LicenseNumber = LabTechnicianLicenseNumber(),
                        LicenseIssuedBy = labTechnicianRegistrationDto.LicenseIssuedBy,
                        LicenseExpiryDate = labTechnicianRegistrationDto.LicenseExpiryDate,
                        Specialization = string.Join(",", labTechnicianRegistrationDto.Specializations),
                        JoiningDate = labTechnicianRegistrationDto.JoiningDate
                    };
                    await _hmsDBContext.LabTechnician_tbl.AddAsync(newUser);
                    await _hmsDBContext.SaveChangesAsync();
                    return "Lab Technician Registration successful";
                }
                else
                {
                    throw new Exception("Lab Technician Registration failed");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Exception: {ex.Message}");
            }
        }
        private string LabTechnicianLicenseNumber()
        {
            // Current year
            int year = DateTime.Now.Year;

            // Random 5-digit number
            Random rnd = new Random();
            int randomNumber = rnd.Next(10000, 99999);

            // Format: CHEM/YEAR/RANDOMNUMBER
            return $"LTECH/{year}/{randomNumber}";
        }
        public async Task<string> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
        {
            try
            {
                var existingUser = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);
                if (existingUser == null)
                {
                    throw new ArgumentException($"User not found with email : {forgotPasswordDto.Email}");
                }
                var token = await _userManager.GeneratePasswordResetTokenAsync(existingUser);
                var resetToken = $"token={HttpUtility.UrlEncode(token)}";

                var body = $"<h3>Reset your password</h3><p>Click here to reset your password: <a href='{resetToken}'>Reset Link</a></p>";

                var isSuccess = await _emailService.SendEmailAsync(forgotPasswordDto.Email, "Password Reset Request", body);
                if (!isSuccess)
                {
                    throw new Exception("Failed to send password reset email.");
                }
                return "Password reset link sent to your email.";
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<string> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            try
            {
                if (resetPasswordDto == null)
                {
                    throw new ArgumentNullException("Parameters can not be null.");
                }
                var existingUser = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
                if (existingUser == null)
                {
                    throw new ArgumentException($"User not found with email : {resetPasswordDto.Email}");
                }
                var decodedToken = HttpUtility.UrlDecode(resetPasswordDto.Token);

                var result = await _userManager.ResetPasswordAsync(existingUser, decodedToken, resetPasswordDto.NewPassword);
                if (!result.Succeeded)
                {
                    throw new Exception("Reset password failed");
                }
                return "Password Reset Successfully";
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
