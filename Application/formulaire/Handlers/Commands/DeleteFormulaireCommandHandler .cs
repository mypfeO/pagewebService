using Application.Eroors;
using Application.formulaire.Commands;
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
    public class DeleteFormulaireCommandHandler : IRequestHandler<DeleteFormulaireCommand, Result>
    {
        private readonly IRepositoryFormulaire _repositoryFormulaire;

        public DeleteFormulaireCommandHandler(IRepositoryFormulaire repositoryFormulaire)
        {
            _repositoryFormulaire = repositoryFormulaire;
        }

        public async Task<Result> Handle(DeleteFormulaireCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (!ObjectId.TryParse(request.Id, out ObjectId formulaireId))
                {
                    return EroorsHandler.HandleGenericError("Invalid Formulaire ID format.");
                }

                var existingFormulaire = await _repositoryFormulaire.GetFormulaireByIdAsync(formulaireId, cancellationToken);
                if (existingFormulaire == null)
                {
                    return EroorsHandler.HandleGenericError("Formulaire not found.");
                }

                var result = await _repositoryFormulaire.DeleteFormulaireAsync(formulaireId, cancellationToken);

                if (result.IsSuccess)
                {
                    return Result.Ok();
                }
                else
                {
                    return EroorsHandler.HandleGenericError($"Error: {result.Errors.First().Message}");
                }
            }
            catch (Exception ex)
            {
                return EroorsHandler.HandleGenericError($"Unexpected error: {ex.Message}");
            }
        }
    }
}
