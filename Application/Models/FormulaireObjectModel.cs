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
        public string SiteWebId { get; set; }
        public Formulaire Formulaire { get; set; }
        public string ExcelFileLink { get; set; }
        public Design Design { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<FormulaireObjectDTO, FormulaireObjectModel>()
                   .ForMember(dest => dest.SiteWebId, opt => opt.MapFrom(src => src.SiteWebId.ToString()))
                   .ForMember(dest => dest.ExcelFileLink, opt => opt.MapFrom(src => src.ExcelFileLink))
                   .ForMember(dest => dest.Design, opt => opt.MapFrom(src => src.Design));
        }
    }




}
