using Application.Models;
using AutoMapper;
using Domaine.Entities;

namespace Application.Common.Mappings
{
    public class FormulaireMappingProfile : Profile
    {
        public FormulaireMappingProfile()
        {
            // Mapping for FormulaireObjectDTO to FormulaireObjectModel
            CreateMap<FormulaireObjectDTO, FormulaireObjectModel>()
                .ForMember(dest => dest.SiteWebId, opt => opt.MapFrom(src => src.SiteWebId.ToString()))
                .ForMember(dest => dest.ExcelFileLink, opt => opt.MapFrom(src => src.ExcelFileLink));
            // Mapping for FormulaireObjectDTO to GetFormsById
            CreateMap<FormulaireObjectDTO, GetFormsById>()
                .ForMember(dest => dest.SiteWebId, opt => opt.MapFrom(src => src.SiteWebId.ToString()))
                .ForMember(dest => dest.ExcelFileLink, opt => opt.MapFrom(src => src.ExcelFileLink))
                .ForMember(dest => dest.Design, opt => opt.MapFrom(src => new DesignSummary
                {
                    ProductImages = src.Design.ProductImages,
                    BackgroundColor = src.Design.BackgroundColor,
                    Logo = src.Design.Logo
                }));

            // Bidirectional mapping for Formulaire and FormulaireDTO
            CreateMap<Formulaire, FormulaireDTO>()
                .ForMember(dest => dest.Head, opt => opt.MapFrom(src => src.Head))
                .ForMember(dest => dest.Body, opt => opt.MapFrom(src => src.Body))
                .ForMember(dest => dest.Footer, opt => opt.MapFrom(src => src.Footer))
                .ReverseMap();

            // Bidirectional mapping for Head and HeadDTO
            CreateMap<Head, HeadDTO>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ReverseMap();

            // Bidirectional mapping for BodyItem and BodyItemDTO
            CreateMap<BodyItem, BodyItemDTO>()
                .ForMember(dest => dest.Titre, opt => opt.MapFrom(src => src.Titre))
                .ForMember(dest => dest.ChampText, opt => opt.MapFrom(src => src.ChampText))
                .ForMember(dest => dest.ImageLink, opt => opt.MapFrom(src => src.ImageLink))
                .ReverseMap();

            // Bidirectional mapping for Footer and FooterDTO
            CreateMap<Footer, FooterDTO>()
                .ForMember(dest => dest.Titre, opt => opt.MapFrom(src => src.Titre))
                .ReverseMap();
        }
    }
}
