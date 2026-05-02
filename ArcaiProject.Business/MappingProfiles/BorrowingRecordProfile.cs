using AutoMapper;
using ArcaiProject.Business.DTOs;
using ArcaiProject.Entities.Entities;

namespace ArcaiProject.Business.MappingProfiles
{
    public class BorrowingRecordProfile : Profile
    {
        public BorrowingRecordProfile()
        {
            CreateMap<BorrowingRecord, BorrowingRecordDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<CreateBorrowingRequestDto, BorrowingRecord>();
        }
    }
}
