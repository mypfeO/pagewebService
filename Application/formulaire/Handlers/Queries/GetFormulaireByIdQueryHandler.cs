using Application.formulaire.Queries;
using Application.Models;
using Domaine.Entities;
using Domaine.Reposotires;
using MediatR;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.formulaire.Handlers.Queries
{
    public class GetFormulaireByIdQueryHandler : IRequestHandler<GetFormulaireByIdQuery, FormulaireObjectDTO>
    {
        private readonly IRepositoryFormulaire _repositoryFormulaire;

        public GetFormulaireByIdQueryHandler(IRepositoryFormulaire repositoryFormulaire)
        {
            _repositoryFormulaire = repositoryFormulaire;
        }

        public async Task<FormulaireObjectDTO> Handle(GetFormulaireByIdQuery request, CancellationToken cancellationToken)
        {
            var objectId = ObjectId.Parse(request.FormulaireId);
            return await _repositoryFormulaire.GetFormulaireByIdAsync(objectId, cancellationToken);
        }
    }

}
