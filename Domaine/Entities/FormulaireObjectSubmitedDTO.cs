using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domaine.Entities
{
    public class FormulaireObjectSubmitedDTO
    {

        //public ObjectId SiteWebId { get; set; }
        //public ObjectId FormId { get; set; } // Added to uniquely identify the form being submitted
        //public ObjectId UserId { get; set; } // To track the submitting user
        public string? ExcelFileLink { get; set; }
        public FormulaireSubmitedDTO? Formulaire { get; set; }
    }

    public class FormulaireSubmitedDTO
    {
        public HeadSubmitedDTO? Head { get; set; }
        public List<IBodyItemSubmitedDTO> Body { get; set; } = new List<IBodyItemSubmitedDTO>();
        public FooterSubmitedDTO? Footer { get; set; }
    }


    public interface IBodyItemSubmitedDTO
    {
        string Titre { get; set; }
        string Type { get; } // To distinguish between different types
    }


    public class HeadSubmitedDTO
        {
            public string Title { get; set; } = string.Empty;
              public string Respense { get; set; } = string.Empty;
    }
    public class TextBodyItemSubmitedDTO : IBodyItemSubmitedDTO
    {
        public string Titre { get; set; } = string.Empty;
        public string Respense { get; set; } = string.Empty;
        public string Type { get; } = "text";
    }

    //public class ImageBodyItemSubmitedDTO : IBodyItemSubmitedDTO
    //{
    //    public string Titre { get; set; } = string.Empty;
    //    public string ImageUrl { get; set; } = string.Empty; // URL to the image
    //    public string Type { get; } = "image";
    //}

    //public class VideoBodyItemSubmitedDTO : IBodyItemSubmitedDTO
    //{
    //    public string Titre { get; set; } = string.Empty;
    //    public string VideoUrl { get; set; } = string.Empty; // URL to the video
    //    public string Type { get; } = "video";
    //}

    public class FooterSubmitedDTO
        {
             public string Titre { get; set; } = string.Empty;
            public string Respense { get; set; } = string.Empty;
    }
    
}
