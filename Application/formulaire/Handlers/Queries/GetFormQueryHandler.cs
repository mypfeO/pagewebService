using Application.formulaire.Queries;
using Application.Models;
using AutoMapper;
using Domaine.Entities;
using Domaine.Reposotires;
using FluentResults;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.formulaire.Handlers.Queries
{
    public class GetFormQueryHandler : IRequestHandler<GetFormQuery, Result<FormulaireObjectModel>>
    {
        private readonly IRepositoryFormulaire _repository;
        private readonly IMapper _mapper;

        public GetFormQueryHandler(IRepositoryFormulaire repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<FormulaireObjectModel>> Handle(GetFormQuery request, CancellationToken cancellationToken)
        {
            var formulaireDto = await _repository.GetFormAsync(request.SiteWebId,request.FormId, cancellationToken);

            if (formulaireDto == null)
            {
                return Result.Fail<FormulaireObjectModel>("Form not found");
            }

            
            var formulaireModel = _mapper.Map<FormulaireObjectModel>(formulaireDto);

            return Result.Ok(formulaireModel);
        }
    }
}
