using AutoMapper;
using ArcaiProject.Business.DTOs;
using ArcaiProject.Entities.Entities;

namespace ArcaiProject.Business.MappingProfiles
{
    public class CourseProfile : Profile
    {
        public CourseProfile()
        {
            CreateMap<Course, CourseDto>();
            CreateMap<CreateCourseDto, Course>();
            CreateMap<UpdateCourseDto, Course>();
        }
    }
}
