using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domaine.Entities
{
    public class FormulaireObjectDTO
    {
        public ObjectId _id { get; set; }
        public ObjectId SiteWebId { get; set; } 
        public FormulaireDTO? Formulaire { get; set; }
    }
    
    public class FormulaireDTO
    {
        public HeadDTO? Head { get; set; }
        public List<BodyItemDTO> Body { get; set; } = new List<BodyItemDTO>();
        public FooterDTO? Footer { get; set; }
    }

          public class HeadDTO
    {
           public string Title { get; set; } = string.Empty;
          }

    public class BodyItemDTO
    {
        public string Titre { get; set; } = string.Empty;
        public bool ChampText { get; set; } = false;
        public string ImageLink { get; set; } = string.Empty;
    }

    public class FooterDTO
    {
        public string Titre { get; set; } = string.Empty;
    }

}
