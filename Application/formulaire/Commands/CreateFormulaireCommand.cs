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
    public class CreateFormulaireCommand : IRequest<Result<string>>
    {
        public string SiteWebId { get; set; } = string.Empty;
        public Formulaire Formulaire { get; set; } = new();
        public string ExcelFileLink { get; set; } = string.Empty;
        public DesignSummary Design { get; set; }
    }
}
