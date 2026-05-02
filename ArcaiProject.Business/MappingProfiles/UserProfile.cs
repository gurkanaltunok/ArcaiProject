using AutoMapper;
using ArcaiProject.Business.DTOs;
using ArcaiProject.Entities.Entities;

namespace ArcaiProject.Business.MappingProfiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserDto>();
            // CreateUserDto'dan User'a mapping. Password hash'leneceği için ignore ediliyor.
            CreateMap<CreateUserDto, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.DocumentsAdded, opt => opt.Ignore())
                .ForMember(dest => dest.RequestedRecords, opt => opt.Ignore())
                .ForMember(dest => dest.ApprovedRecords, opt => opt.Ignore());
        }
    }
}
