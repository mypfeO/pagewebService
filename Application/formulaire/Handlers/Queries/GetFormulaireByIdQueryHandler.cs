using Application.formulaire.Queries;
using Domaine.Entities;
using Domaine.Reposotires;
using MediatR;
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
            return await _repositoryFormulaire.GetFormulaireByIdAsync(request.FormulaireId, cancellationToken);
        }
    }

}
