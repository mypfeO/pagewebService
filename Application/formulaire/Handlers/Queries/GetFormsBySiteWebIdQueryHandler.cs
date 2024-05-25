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
            var validator = new GetFormsBySiteWebIdQueryValidator();
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return EroorsHandler.HandleGenericError<List<FormulaireObjectModel>>(string.Join(", ", errors));
            }

            try
            {
                var siteWebId = ObjectId.Parse(request.SiteWebId);
                var formulaires = await _repository.GetFormsBySiteWebIdAsync(siteWebId, cancellationToken);

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
