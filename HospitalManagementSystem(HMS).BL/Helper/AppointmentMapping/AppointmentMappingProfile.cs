using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using HospitalManagementSystem_HMS_.DB.Model.AppModel.AppointmentModel;
using HospitalManagementSystem_HMS_.Dtos.AppDtos.AdminDtos;
using HospitalManagementSystem_HMS_.Dtos.AppDtos.DoctorDtos;
using HospitalManagementSystem_HMS_.Dtos.AppDtos.PatientDtos;

namespace HospitalManagementSystem_HMS_.BL.Helper.AppointmentMapping
{
    public class AppointmentMappingProfile:Profile
    {
        public AppointmentMappingProfile()
        {
            CreateMap<PrescriptionDetail , PrescriptionDetailDto>()
                .ForMember(dest => dest.MedicineName, opt => opt.MapFrom(src => src.MedicineName))
                .ForMember(dest => dest.Dosage, opt => opt.MapFrom(src => src.Dosage))
                .ForMember(dest => dest.Frequency, opt => opt.MapFrom(src => src.Frequency))
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.Duration))
                .ForMember(dest => dest.Instructions, opt => opt.MapFrom(src => src.Instructions))
                .ReverseMap();
            
            CreateMap<Prescription , CreatePrescriptionDto>()
                .ForMember(dest => dest.AppointmentId, opt => opt.MapFrom(src => src.AppointmentId))
                .ForMember(dest => dest.Note, opt => opt.MapFrom(src => src.Note))
                .ForMember(dest => dest.Medicines, opt => opt.MapFrom(src => src.PrescriptionDetails))
                .ReverseMap();

            CreateMap<PrescriptionDetail, MedicineInfoResponseDto>()
            .ForMember(dest => dest.MedicineName, opt => opt.MapFrom(src => src.MedicineName))
            .ForMember(dest => dest.Dosage, opt => opt.MapFrom(src => src.Dosage))
            .ForMember(dest => dest.Frequency, opt => opt.MapFrom(src => src.Frequency))
            .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.Duration))
            .ForMember(dest => dest.Instructions, opt => opt.MapFrom(src => src.Instructions))
            .ReverseMap();

            CreateMap<Prescription, PrescriptionResponseDto>()
              .ForMember(dest => dest.PrescriptionId, opt => opt.MapFrom(src => src.PrescriptionId))
              .ForMember(dest => dest.AppointmentId, opt => opt.MapFrom(src => src.AppointmentId))
              .ForMember(dest => dest.PrescribeDate, opt => opt.MapFrom(src => src.PrescribeDate))
              .ForMember(dest => dest.MedicinesInfoDto, opt => opt.MapFrom(src => src.PrescriptionDetails))
              .ReverseMap();

            CreateMap<LabTests, LabTestsDto>()
                .ReverseMap();
                
        }
    }
}
