using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
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
        public FormulaireDTO Formulaire { get; set; }
        public string ExcelFileLink { get; set; }
        public string? CodeBoard { get; set; }// Add this line
        public DesignDTO Design { get; set; }
    }

    public class FormulaireDTO
    {
        public HeadDTO Head { get; set; }
        public List<BodyItemDTO> Body { get; set; } = new List<BodyItemDTO>();
        public List<FooterItemDTO> Footer { get; set; } = new List<FooterItemDTO>();
    }


    public class HeadDTO
            {
           public string Title { get; set; } = string.Empty;
          }

    public class BodyItemDTO
    {
        public string Titre { get; set; } = string.Empty;
        public String Type { get; set; }
        public string RespenseText { get; set; } = string.Empty;
        public bool Required { get; set; } = false;
    }

   

    public class FooterItemDTO
    {
        public string Titre { get; set; }
        public string LinkNextForm { get; set; }
    }


    public class DesignDTO
    {
        public List<string> ProductImages { get; set; }
        public string BackgroundColor { get; set; }
        public string Logo { get; set; }
    }
}
