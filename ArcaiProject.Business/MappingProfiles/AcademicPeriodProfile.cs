using AutoMapper;
using ArcaiProject.Business.DTOs;
using ArcaiProject.Entities.Entities;

namespace ArcaiProject.Business.MappingProfiles
{
    public class AcademicPeriodProfile : Profile
    {
        public AcademicPeriodProfile()
        {
            CreateMap<AcademicPeriod, AcademicPeriodDto>();
            CreateMap<CreateAcademicPeriodDto, AcademicPeriod>();
            CreateMap<UpdateAcademicPeriodDto, AcademicPeriod>();
        }
    }
}
