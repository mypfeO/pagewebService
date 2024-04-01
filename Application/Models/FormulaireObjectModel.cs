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


    }




}
