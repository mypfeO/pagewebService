using Application.Common.Mappings;
using Domaine.Entities;
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
        public List<BodyItem> Body { get; set; } = new List<BodyItem>();
        public Footer? Footer { get; set; }
       
    }

    public class Head : IMapFrom<HeadDTO>
    {
        public string Title { get; set; } = string.Empty;
    }

    public class BodyItem : IMapFrom<BodyItemDTO> 
    {
        public string Titre { get; set; } = string.Empty;
        public bool ChampText { get; set; } = false;
        public string ImageLink { get; set; } = string.Empty;
    }

    public class Footer : IMapFrom<FooterDTO> 
    {
        public string Titre { get; set; } = string.Empty;
    }


}
