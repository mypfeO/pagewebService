using Application.Eroors;
using Application.formulaire.Commands;
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

namespace Application.formulaire.Handlers.Commands
{
    public class CreateFormulaireCommandHandler : IRequestHandler<CreateFormulaireCommand, Result<string>>
    {
        private readonly IRepositoryFormulaire _repositoryFormulaire;
        private readonly IMapper _mapper;

        public CreateFormulaireCommandHandler(IRepositoryFormulaire repositoryFormulaire, IMapper mapper)
        {
            _repositoryFormulaire = repositoryFormulaire;
            _mapper = mapper;
        }

        public async Task<Result<string>> Handle(CreateFormulaireCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (!ObjectId.TryParse(request.SiteWebId, out ObjectId siteWebObjectId))
                {
                    return EroorsHandler.HandleGenericError("Format SiteWebId invalide.");
                }

                var formulaireDTO = _mapper.Map<FormulaireDTO>(request.Formulaire);

                var formulaireModel = new FormulaireObjectDTO
                {
                    SiteWebId = siteWebObjectId,
                    Formulaire = formulaireDTO
                };

                var result = await _repositoryFormulaire.AddFormulaireAsync(formulaireModel, cancellationToken);

                if (result.IsSuccess)
                {
                    return Result.Ok("Le formulaire a été créé avec succès.");
                }
                else
                {
                    return EroorsHandler.HandleGenericError($"Erreur lors de la création du formulaire. Raison : {result.Errors.First().Message}");
                }
            }
            catch (Exception ex)
            {
                return EroorsHandler.HandleGenericError($"Erreur inattendue : {ex.Message}");
            }
        }

    }


}
