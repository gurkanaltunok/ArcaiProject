using AutoMapper;
using ArcaiProject.Entities.Entities;
using ArcaiProject.Business.DTOs;

namespace ArcaiProject.Business.MappingProfiles
{
    public class NotificationProfile : Profile
    {
        public NotificationProfile()
        {
            CreateMap<Notification, NotificationDto>();
        }
    }
}
