using Application.Models;
using AutoMapper;
using Domaine.Entities;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Mappings
{
    public class FormulaireMappingProfile : Profile
    {
        public FormulaireMappingProfile()
        {
            
            CreateMap<FormulaireObjectDTO, FormulaireObjectModel>()
                .ForMember(dest => dest.SiteWebId, opt => opt.MapFrom(src => src.SiteWebId.ToString()))
                .ForMember(dest => dest.ExcelFileLink, opt => opt.MapFrom(src => src.ExcelFileLink.ToString()));


            CreateMap<Formulaire, FormulaireDTO>()
                .ForMember(dest => dest.Head, opt => opt.MapFrom(src => src.Head))
                .ForMember(dest => dest.Body, opt => opt.MapFrom(src => src.Body))
                .ForMember(dest => dest.Footer, opt => opt.MapFrom(src => src.Footer));

            CreateMap<Head, HeadDTO>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title));

            CreateMap<BodyItem, BodyItemDTO>()
                .ForMember(dest => dest.Titre, opt => opt.MapFrom(src => src.Titre))
                .ForMember(dest => dest.ChampText, opt => opt.MapFrom(src => src.ChampText))
                .ForMember(dest => dest.ImageLink, opt => opt.MapFrom(src => src.ImageLink));

            CreateMap<Footer, FooterDTO>()
                .ForMember(dest => dest.Titre, opt => opt.MapFrom(src => src.Titre));

            CreateMap<FormulaireSummaryDTO, FormulaireSummary>()
            .ForMember(dest => dest.FormulaireId, opt => opt.MapFrom(src => src.FormulaireId));
        }
    }

}
