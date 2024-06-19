using Application.Common.Mappings;
using AutoMapper;
using Domaine.Entities;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{

    public class FormulaireObjectModel : IMapFrom<FormulaireObjectDTO>
    {
        public string _id { get; set; } = string.Empty; // Added field for `_id`
        public string SiteWebId { get; set; } = string.Empty;
        public Formulaire Formulaire { get; set; } = new();
        public string ExcelFileLink { get; set; } = string.Empty;
        public string CodeBoard { get; set; } = string.Empty;
        public DesignSummary Design { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<FormulaireObjectDTO, FormulaireObjectModel>()
                   .ForMember(dest => dest.SiteWebId, opt => opt.MapFrom(src => src.SiteWebId.ToString()))
                   .ForMember(dest => dest.ExcelFileLink, opt => opt.MapFrom(src => src.ExcelFileLink))
                     .ForMember(dest => dest.CodeBoard, opt => opt.MapFrom(src => src.CodeBoard)) // Add this line

                   .ForMember(dest => dest.Design, opt => opt.MapFrom(src => src.Design)).ForMember(dest => dest.Design, opt => opt.MapFrom(src => new DesignSummary
                   {
                       ProductImages = src.Design.ProductImages,
                       BackgroundColor = src.Design.BackgroundColor,
                       Logo = src.Design.Logo
                   }));
        }
    }
}
