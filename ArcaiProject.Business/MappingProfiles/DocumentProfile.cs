using AutoMapper;
using ArcaiProject.Business.DTOs;
using ArcaiProject.Entities.Entities;
using System.Linq;

namespace ArcaiProject.Business.MappingProfiles
{
    public class DocumentProfile : Profile
    {
        public DocumentProfile()
        {
            CreateMap<Document, DocumentDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.DocumentTags.Select(dt => dt.Tag)));

            CreateMap<CreateDocumentDto, Document>();
            CreateMap<UpdateDocumentDto, Document>();
        }
    }
}
