using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using HospitalManagementSystem_HMS_.DB.Model.AppModel;
using HospitalManagementSystem_HMS_.Dtos.AppDtos;

namespace HospitalManagementSystem_HMS_.BL.Helper.PatientMapping
{
    public class PatientMappingProfile:Profile
    {
        public PatientMappingProfile()
        {
            CreateMap<PatientDetails , PatientDetailsDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.AppUser.Name))
                .ForMember(dest => dest.AadharNumber, opt => opt.MapFrom(src => src.AppUser.AadharNumber))
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.AppUser.Age))
                .ForMember(dest=>dest.Gender, opt => opt.MapFrom(src => src.AppUser.Gender))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.AppUser.Email))
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.AppUser.DateOfBirth))
                .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.AppUser.State))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.AppUser.City))
                .ForMember(dest => dest.PostalCode, opt => opt.MapFrom(src => src.AppUser.PostalCode))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.AppUser.PhoneNumber))
                .ReverseMap(); 
        }
    }
}
