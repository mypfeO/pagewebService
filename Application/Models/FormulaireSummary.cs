using Application.Common.Mappings;
using Domaine.Entities;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public class FormulaireSummary : IMapFrom<FormulaireSummaryDTO>
    {
        public string FormulaireId { get; set; }
        public string Title { get; set; }
     
    }

}
