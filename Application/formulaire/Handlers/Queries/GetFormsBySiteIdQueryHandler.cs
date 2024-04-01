using Application.formulaire.Queries;
using Application.Models;
using AutoMapper;
using Domaine.Entities;
using Domaine.Reposotires;
using FluentResults;
using MediatR;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.formulaire.Handlers.Queries
{
    public class GetFormsBySiteIdQueryHandler : IRequestHandler<GetFormsBySiteIDQuery, Result<FormulaireSummary>>
    {
        private readonly IRepositoryFormulaire _repositoryFormulaire;
        private readonly IMapper _mapper;
        public GetFormsBySiteIdQueryHandler(IRepositoryFormulaire repositoryFormulaire, IMapper mapper)
        {
            _repositoryFormulaire = repositoryFormulaire;
            _mapper = mapper;
        }
        public async Task<Result<FormulaireSummary>> Handle(GetFormsBySiteIDQuery request, CancellationToken cancellationToken)
        {
            if (!ObjectId.TryParse(request.Id, out ObjectId siteWebObjectId))
            {
                return Result.Fail<FormulaireSummary>("Invalid SiteWebId format.");
            }

            var result = await _repositoryFormulaire.GetFormsBySiteIdAsync(request.Id, cancellationToken);

            if (result == null || result.Count == 0)
            {
                Console.WriteLine("No forms found for site ID: " + request.Id);
                return Result.Fail<FormulaireSummary>("No forms found");
            }

            var formulaireSummaryDTO = result.First();
            var formulaireModel = _mapper.Map<FormulaireSummary>(formulaireSummaryDTO);

            return Result.Ok(formulaireModel);
        }


    }
}
