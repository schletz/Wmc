using AutoMapper;
using System;
using System.Linq;
using WmcApi.Model;

namespace WmcApi.Dto
{
    public class DtoMappingProfile : Profile
    {
        public DtoMappingProfile()
        {
            CreateMap<Student, StudentDto>();
            CreateMap<Student, StudentDetailsDto>()
                .ForCtorParam(nameof(StudentDetailsDto.DateOfBirth), opt => opt.MapFrom(src => src.DateOfBirth.ToString("yyyy-MM-dd")));
            CreateMap<Schoolclass, SchoolclassDto>()
                .ForCtorParam(nameof(SchoolclassDto.StudentCount), opt => opt.MapFrom(src => src.Students.Count()))
                .ForCtorParam(nameof(SchoolclassDto.StudentMaleCount), opt => opt.MapFrom(src => src.Students.Count(s => s.Gender == Gender.Male)))
                .ForCtorParam(nameof(SchoolclassDto.StudentFemaleCount), opt => opt.MapFrom(src => src.Students.Count(s => s.Gender == Gender.Female)));
            CreateMap<Schoolclass, SchoolclassDetailsDto>();
            CreateMap<Period, PeriodDto>()
                .ForCtorParam(nameof(PeriodDto.Duration), opt => opt.MapFrom(src => (int)(src.End - src.Start).TotalMinutes))
                .ForCtorParam(nameof(PeriodDto.Start), opt => opt.MapFrom(src => src.Start.ToString(@"hh\:mm")))
                .ForCtorParam(nameof(PeriodDto.End), opt => opt.MapFrom(src => src.End.ToString(@"hh\:mm")));
            CreateMap<Teacher, TeacherDto>();
            CreateMap<Room, RoomDto>();
        }
    }
}