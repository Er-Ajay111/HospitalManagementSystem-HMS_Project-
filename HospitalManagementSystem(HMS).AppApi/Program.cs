using HospitalManagementSystem_HMS_.AppApi.Extension;
using HospitalManagementSystem_HMS_.BL.AppRepo.Admin.Implementation;
using HospitalManagementSystem_HMS_.BL.AppRepo.Admin.IServices;
using HospitalManagementSystem_HMS_.BL.AppRepo.Doctor.Implementation;
using HospitalManagementSystem_HMS_.BL.AppRepo.Doctor.IServices;
using HospitalManagementSystem_HMS_.BL.AppRepo.LabTechnician.Implementation;
using HospitalManagementSystem_HMS_.BL.AppRepo.LabTechnician.IServices;
using HospitalManagementSystem_HMS_.BL.AppRepo.Patient.Implementation;
using HospitalManagementSystem_HMS_.BL.AppRepo.Patient.IServices;
using HospitalManagementSystem_HMS_.BL.AppRepo.Utility.Implementation;
using HospitalManagementSystem_HMS_.BL.AppRepo.Utility.IService;
using HospitalManagementSystem_HMS_.BL.AuthRepo.Implementation;
using HospitalManagementSystem_HMS_.BL.AuthRepo.IServices;
using HospitalManagementSystem_HMS_.BL.Helper.DoctorMapping;
using HospitalManagementSystem_HMS_.DB.Data;
using HospitalManagementSystem_HMS_.DB.Model.AuthModel;
using HospitalManagementSystem_HMS_.Dtos.AuthDtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Infrastructure;

namespace HospitalManagementSystem_HMS_.AppApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            QuestPDF.Settings.License = LicenseType.Community;
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<HMSDBContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("dbcs"),
                    sqlOptions => sqlOptions.MigrationsAssembly("HospitalManagementSystem(HMS).AppApi"));
            });
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
              .AddEntityFrameworkStores<HMSDBContext>()
              .AddDefaultTokenProviders();
            builder.Services.AddAuthentication();
            builder.AddTokenConfiguration();
            builder.Services.AddScoped<IDoctorService, DoctorService>();
            builder.Services.AddScoped<IPatientService, PatientService>();
            builder.Services.AddScoped<IAdminService, AdminService>();
            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<IPdfService, PdfService>();
            builder.Services.AddScoped<ILabTechnicianService, LabTechnicianService>();
            builder.Services.AddHttpContextAccessor();

            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            var app = builder.Build();
            app.UseStaticFiles();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
