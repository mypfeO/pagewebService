using Application.Common.Mappings;
using Domaine.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public class Formulaire : IMapFrom<FormulaireDTO>
    {
        public Head? Head { get; set; }
        public List<BodyItem>? Body { get; set; }
        public List<string>? Bodies { get; set; }
        public Footer? Footer { get; set; }
        public List<IFormFile> ProductImages { get; set; }
    }

    public class Head : IMapFrom<HeadDTO>
    {
        public string Title { get; set; } = string.Empty;
    }

    public class BodyItem : IMapFrom<BodyItemDTO> 
    {
        public string Titre { get; set; } = string.Empty;
        public bool ChampText { get; set; } = false;
        public bool ImageLink { get; set; } = false;

        public bool required { get; set; } = false;

    }

    public class Footer : IMapFrom<FooterDTO> 
    {
        public string Titre { get; set; } = string.Empty;
    }


}
