using Domaine.Entities;
using Domaine.Reposotires;
using FluentResults;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class MongoRepositoryFormulaire : IRepositoryFormulaire
    {
        private readonly IMongoCollection<FormulaireObjectDTO> _formulaireCollection;

        public MongoRepositoryFormulaire(IMongoDatabase database)
        {
            _formulaireCollection = database.GetCollection<FormulaireObjectDTO>("formulaires");
        }

        public async Task<Result> AddFormulaireAsync(FormulaireObjectDTO formulaire, CancellationToken cancellationToken)
        {
            try
            {
                await _formulaireCollection.InsertOneAsync(formulaire, null, cancellationToken);
                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail($"Erreur lors de l'ajout du formulaire : {ex.Message}");
            }
        }

        public async Task<List<FormulaireSummaryDTO>> GetFormsBySiteIdAsync(string siteWebId, CancellationToken cancellationToken)
        {
            var SiteWebId = ObjectId.Parse(siteWebId);
           
            var filter = Builders<FormulaireObjectDTO>.Filter.Eq(x => x.SiteWebId, SiteWebId);
          
            var projection = Builders<FormulaireObjectDTO>.Projection
                .Include(x => x._id)
                .Include(x => x.Formulaire.Head.Title);

            var formulaires = await _formulaireCollection.Find(filter)
                .Project<FormulaireObjectDTO>(projection)
                .ToListAsync(cancellationToken);

            var formulaireSummaryDTOs = formulaires.Select(dto => new FormulaireSummaryDTO
            {
                FormulaireId = dto._id.ToString(), 
                Title = dto.Formulaire?.Head?.Title
            }).ToList();

            return formulaireSummaryDTOs;
        }

        public async Task<FormulaireObjectDTO> GetFormulaireByIdAsync(string formulaireId, CancellationToken cancellationToken)
        {
            var objectId = ObjectId.Parse(formulaireId);
            var filter = Builders<FormulaireObjectDTO>.Filter.Eq(x => x._id, objectId);

            return await _formulaireCollection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        }
    }
}
