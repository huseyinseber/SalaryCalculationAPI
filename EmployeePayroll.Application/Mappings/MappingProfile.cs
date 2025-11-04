// EmployeePayroll.Application/Mappings/MappingProfile.cs
using AutoMapper;
using EmployeePayroll.Application.DTOs;
using EmployeePayroll.Core.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EmployeePayroll.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Employee, EmployeeDto>().ReverseMap();
            CreateMap<Employee, CreateEmployeeDto>().ReverseMap();

            CreateMap<WorkLog, WorkLogDto>().ReverseMap();
            CreateMap<WorkLog, CreateWorkLogDto>().ReverseMap();

            CreateMap<Employee, PayrollDto>()
                .ForMember(dest => dest.EmployeeName,
                           opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
                .ForMember(dest => dest.MaskedTCKN,
                           opt => opt.MapFrom(src => MaskTCKN(src.TCKN)));
        }

        private static string MaskTCKN(string tckn)
        {
            if (string.IsNullOrEmpty(tckn) || tckn.Length < 4)
                return tckn;

            return new string('*', tckn.Length - 4) + tckn.Substring(tckn.Length - 4);
        }
    }
}