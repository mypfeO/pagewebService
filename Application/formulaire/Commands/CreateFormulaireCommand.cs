using Application.Models;
using FluentResults;
using MediatR;
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
    }

    // Gestionnaire
   

}
