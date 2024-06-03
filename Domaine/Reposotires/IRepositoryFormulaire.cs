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
        Task<Result<string>> AddFormulaireAsync(FormulaireObjectDTO formulaire, CancellationToken cancellationToken);
        Task<FormulaireObjectDTO> GetFormulaireByIdAsync(ObjectId formulaireId, CancellationToken cancellationToken);
        Task<FormulaireObjectDTO> GetFormulaireAsync(ObjectId siteWebId, ObjectId formId, CancellationToken cancellationToken);
        Task<Result<string>> SaveFormAsync(FormulaireObjectSubmitedDTO formulaireSubmited, CancellationToken cancellationToken);
       
        Task<Result> UpdateFormulaireAsync(FormulaireObjectDTO formulaire, CancellationToken cancellationToken);
        Task<Result> DeleteFormulaireAsync(ObjectId formulaireId, CancellationToken cancellationToken);
        Task<List<FormulaireObjectDTO>> GetFormsBySiteWebIdAsync(ObjectId siteWebId, CancellationToken cancellationToken);


    }

}
 