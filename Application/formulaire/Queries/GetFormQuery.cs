using Application.Models;
using Domaine.Entities;
using FluentResults;
using MediatR;

namespace Application.formulaire.Queries
{
    public class GetFormulaireQuery : IRequest<Result<GetFormsById>>
    {
        public string SiteWebId { get; set; }
        public string FormId { get; set; }
    }


}
