using Application.Models;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.formulaire.Commands
{
    // Commande
    public class CreateFormulaireCommand : IRequest<Result<string>>
    {
        public string SiteWebId { get; set; }
        public Formulaire Formulaire { get; set; }
        public string ExcelFileLink { get; set; }
        public List<IFormFile> ProductImages { get; set; }
        public IFormFile Logo { get; set; }
        public string BackgroundColor { get; set; }
    }



    // Gestionnaire


}
