using Domaine.Entities;
using FluentResults;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domaine.Reposotires
{
    public interface IRepositoryFormulaire
    {
        Task<List<FormulaireSummaryDTO>> GetFormsBySiteIdAsync(string siteWebId, CancellationToken cancellationToken);
        Task<Result>  AddFormulaireAsync(FormulaireObjectDTO formulaire, CancellationToken cancellationToken);
        Task<FormulaireObjectDTO> GetFormulaireByIdAsync(string formulaireId, CancellationToken cancellationToken);
    }
    
}
