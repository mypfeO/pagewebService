using Application.Common.Validator;
using Application.Eroors;
using Application.formulaire.Queries;
using Application.Models;
using AutoMapper;
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
    public class GetFormsBySiteWebIdQueryHandler : IRequestHandler<GetFormsBySiteWebIdQuery, Result<List<FormulaireObjectModel>>>
    {
        private readonly IRepositoryFormulaire _repository;
        private readonly IMapper _mapper;

        public GetFormsBySiteWebIdQueryHandler(IRepositoryFormulaire repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<List<FormulaireObjectModel>>> Handle(GetFormsBySiteWebIdQuery request, CancellationToken cancellationToken)
        {
            if (!ObjectId.TryParse(request.SiteWebId, out ObjectId siteWebObjectId))
            {
                return EroorsHandler.HandleGenericError<List<FormulaireObjectModel>>("Invalid SiteWebId format.");
            }

            try
            {
                var formulaires = await _repository.GetFormsBySiteWebIdAsync(siteWebObjectId, cancellationToken);

                if (formulaires == null || !formulaires.Any())
                {
                    return EroorsHandler.HandleGenericError<List<FormulaireObjectModel>>("No forms found for the provided SiteWebId");
                }

                var mappedResult = _mapper.Map<List<FormulaireObjectModel>>(formulaires);
                return Result.Ok(mappedResult);
            }
            catch (Exception ex)
            {
                return EroorsHandler.HandleGenericError<List<FormulaireObjectModel>>($"Unexpected error: {ex.Message}");
            }
        }
    }

}
