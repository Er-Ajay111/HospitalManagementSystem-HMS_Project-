using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using HospitalManagementSystem_HMS_.DB.Model.AppModel;
using HospitalManagementSystem_HMS_.Dtos.AppDtos;
using HospitalManagementSystem_HMS_.Dtos.AppDtos.SearchDto;

namespace HospitalManagementSystem_HMS_.BL.Helper.DoctorMapping
{
    public class DoctorMappingProfile : Profile
    {
        public DoctorMappingProfile()
        {
            CreateMap<DoctorDetails, DoctorDetailsDto>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.AppUser.Name))
            .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.AppUser.Gender))
            .ForMember(dest => dest.AadharNumber, opt => opt.MapFrom(src => src.AppUser.AadharNumber))
            .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.AppUser.Age))
            .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.AppUser.DateOfBirth))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.AppUser.Email))
            .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.AppUser.State))
            .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.AppUser.City))
            .ForMember(dest => dest.PostalCode, opt => opt.MapFrom(src => src.AppUser.PostalCode))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.AppUser.PhoneNumber))
            .ForMember(dest => dest.Qualifications, opt => opt.MapFrom(src => src.Qualifications.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()))
            .ForMember(dest => dest.AvailableDays, opt => opt.MapFrom(src => src.AvailableDays.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()))
            .ForMember(dest => dest.LicenseExpiryDate, opt => opt.MapFrom(src => src.LicenseExpiryDate ?? DateTime.MinValue))
            .ForMember(dest => dest.Specialization, opt => opt.MapFrom(src => src.Specialization))
            .ForMember(dest => dest.ExperienceYears, opt => opt.MapFrom(src => src.ExperienceYears))
            .ForMember(dest => dest.ConsultantFee, opt => opt.MapFrom(src => src.ConsultantFee))
            .ForMember(dest => dest.AlternatePhoneNo, opt => opt.MapFrom(src => src.AlternatePhoneNo))
            .ForMember(dest => dest.LicenseNumber, opt => opt.MapFrom(src => src.LicenseNumber))
            .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.StartTime))
            .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.EndTime))
            .ForMember(dest => dest.RoomNumber, opt => opt.MapFrom(src => src.RoomNumber))
            .ReverseMap();

            CreateMap<DoctorDetails, DoctorDtoForPatientView>()
                .ForMember(dest => dest.DoctorId, opt => opt.MapFrom(src => src.AppUser.Id))
               .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.AppUser.Name))
               .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.AppUser.Email))
               .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.AppUser.State))
               .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.AppUser.City))
               .ForMember(dest => dest.AvailableDays, opt => opt.MapFrom(src => src.AvailableDays.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()))
               .ForMember(dest => dest.Qualifications, opt => opt.MapFrom(src => src.Qualifications.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()))
               .ReverseMap();
        }
    }
}
