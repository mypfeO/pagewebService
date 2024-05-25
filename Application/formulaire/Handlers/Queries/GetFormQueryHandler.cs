using Application.Eroors;
using Application.formulaire.Queries;
using Application.Models;
using AutoMapper;
using Domaine.Entities;
using Domaine.Reposotires;
using FluentResults;
using MediatR;
using MongoDB.Bson;
using System;
using System.Threading;
using System.Threading.Tasks;

public class GetFormulaireQueryHandler : IRequestHandler<GetFormulaireQuery, Result<GetFormsById>>
{
    private readonly IRepositoryFormulaire _repositoryFormulaire;
    private readonly IMapper _mapper;

    public GetFormulaireQueryHandler(IRepositoryFormulaire repositoryFormulaire, IMapper mapper)
    {
        _repositoryFormulaire = repositoryFormulaire;
        _mapper = mapper;
    }

    public async Task<Result<GetFormsById>> Handle(GetFormulaireQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (!ObjectId.TryParse(request.SiteWebId, out ObjectId siteWebObjectId) || !ObjectId.TryParse(request.FormId, out ObjectId formId))
            {
                return EroorsHandler.HandleGenericError<GetFormsById>("Invalid SiteWebId or FormId format.");
            }

            var formulaireDTO = await _repositoryFormulaire.GetFormulaireAsync(siteWebObjectId, formId, cancellationToken);

            if (formulaireDTO == null)
            {
                return EroorsHandler.HandleGenericError<GetFormsById>("Formulaire not found.");
            }

            var formulaireModel = _mapper.Map<GetFormsById>(formulaireDTO);
            return Result.Ok(formulaireModel);
        }
        catch (Exception ex)
        {
            return EroorsHandler.HandleGenericError<GetFormsById>($"Unexpected error: {ex.Message}");
        }
    }
}
