using Application.Models;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.formulaire.Commands
{
    public class UpdateFormulaireCommand : IRequest<Result<string>>
    {
        public string Id { get; set; }
        public string SiteWebId { get; set; }
        public Formulaire Formulaire { get; set; }
        public string ExcelFileLink { get; set; }
        public List<string> ProductImages { get; set; } // Base64 strings
        public string Logo { get; set; } // Base64 string
        public string BackgroundColor { get; set; }
    }
}
 