using AutoMapper;
using ArcaiProject.Business.DTOs;
using ArcaiProject.Entities.Entities;

namespace ArcaiProject.Business.MappingProfiles
{
    public class LocationProfile : Profile
    {
        public LocationProfile()
        {
            CreateMap<Location, LocationDto>();
            CreateMap<CreateLocationDto, Location>();
            CreateMap<UpdateLocationDto, Location>();
        }
    }
}
