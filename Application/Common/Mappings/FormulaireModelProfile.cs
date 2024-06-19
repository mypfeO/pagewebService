using Application.Models;
using AutoMapper;
using Domaine.Entities;
using MongoDB.Bson;

namespace Application.Common.Mappings
{
    public class FormulaireMappingProfile : Profile
    {
        public FormulaireMappingProfile()
        {
            // Mapping for FormulaireObjectDTO to FormulaireObjectModel
            CreateMap<FormulaireObjectDTO, FormulaireObjectModel>()
               .ForMember(dest => dest.SiteWebId, opt => opt.MapFrom(src => src.SiteWebId.ToString()))
                .ForMember(dest => dest.ExcelFileLink, opt => opt.MapFrom(src => src.ExcelFileLink))
                 .ForMember(dest => dest.CodeBoard, opt => opt.MapFrom(src => src.CodeBoard)) // Add this line

                .ForMember(dest => dest.Design, opt => opt.MapFrom(src => new DesignSummary
                {
                    ProductImages = src.Design.ProductImages,
                    BackgroundColor = src.Design.BackgroundColor,
                    Logo = src.Design.Logo
                }));
            // Mapping for FormulaireObjectModel to FormulaireObjectDTO
            CreateMap<FormulaireObjectModel, FormulaireObjectDTO>()
             .ForMember(dest => dest.SiteWebId, opt => opt.MapFrom(src => src.SiteWebId.ToString()))
                .ForMember(dest => dest.ExcelFileLink, opt => opt.MapFrom(src => src.ExcelFileLink))
                  .ForMember(dest => dest.CodeBoard, opt => opt.MapFrom(src => src.CodeBoard)) // Add this line

                .ForMember(dest => dest.Design, opt => opt.MapFrom(src => new DesignSummary
                {
                    ProductImages = src.Design.ProductImages,
                    BackgroundColor = src.Design.BackgroundColor,
                    Logo = src.Design.Logo
                }));







            // Mapping for FormulaireObjectDTO to GetFormsById
            CreateMap<FormulaireObjectDTO, GetFormsById>()
                .ForMember(dest => dest.SiteWebId, opt => opt.MapFrom(src => src.SiteWebId.ToString()))
                .ForMember(dest => dest.ExcelFileLink, opt => opt.MapFrom(src => src.ExcelFileLink))
                .ForMember(dest => dest.CodeBoard, opt => opt.MapFrom(src => src.CodeBoard))
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
            CreateMap<BodyItemDTO, BodyItem>()
               .ForMember(dest => dest.Titre, opt => opt.MapFrom(src => src.Titre))
               .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
               .ForMember(dest => dest.RespenseText, opt => opt.MapFrom(src => src.RespenseText))
               .ForMember(dest => dest.Required, opt => opt.MapFrom(src => src.Required))
               .ReverseMap();

            // Bidirectional mapping for Footer and FooterDTO


     
                // Other mappings
        CreateMap<FooterItemDTO, FooterItem>()
                    .ForMember(dest => dest.Titre, opt => opt.MapFrom(src => src.Titre))
                    .ForMember(dest => dest.LinkNextForm, opt => opt.MapFrom(src => src.LinkNextForm));

              
                CreateMap<FooterItem, FooterItemDTO>()
                    .ForMember(dest => dest.Titre, opt => opt.MapFrom(src => src.Titre))
                    .ForMember(dest => dest.LinkNextForm, opt => opt.MapFrom(src => src.LinkNextForm));
       


    }
}
}
