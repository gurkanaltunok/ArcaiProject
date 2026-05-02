using AutoMapper;
using ArcaiProject.Business.DTOs;
using ArcaiProject.Entities.Entities;

namespace ArcaiProject.Business.MappingProfiles
{
    public class DocumentTypeProfile : Profile
    {
        public DocumentTypeProfile()
        {
            // Entity -> DTO
            CreateMap<DocumentType, DocumentTypeDto>();

            // DTO -> Entity
            CreateMap<CreateDocumentTypeDto, DocumentType>();
            CreateMap<UpdateDocumentTypeDto, DocumentType>();
        }
    }
}
