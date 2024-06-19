using Amazon.Runtime.Internal;
using Domaine.Entities;
using Domaine.Reposotires;
using FluentResults;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Infrastructure.Repositories
{
    public class MongoRepositoryFormulaire : IRepositoryFormulaire
    {
        private readonly IMongoCollection<FormulaireObjectDTO> _formulaireCollection;
        private readonly IMongoCollection<FormulaireObjectSubmitedDTO> _formulairesCollection;

        public MongoRepositoryFormulaire(IMongoDatabase database)
        {
            _formulaireCollection = database.GetCollection<FormulaireObjectDTO>("formulaires");
            _formulairesCollection = database.GetCollection<FormulaireObjectSubmitedDTO>("FormulairesSubmited");
        }

        public async Task<Result<string>> AddFormulaireAsync(FormulaireObjectDTO formulaire, CancellationToken cancellationToken)
        {
            try
            {
                await _formulaireCollection.InsertOneAsync(formulaire, null, cancellationToken);
                return Result.Ok(formulaire._id.ToString()); 
            }
            catch (Exception ex)
            {
                return Result.Fail<string>($"Error adding form: {ex.Message}");
            }
        }

        public async Task<FormulaireObjectDTO> GetFormulaireAsync(ObjectId siteWebId, ObjectId formId, CancellationToken cancellationToken)
        {
            var filter = Builders<FormulaireObjectDTO>.Filter.Eq(f => f.SiteWebId, siteWebId) & Builders<FormulaireObjectDTO>.Filter.Eq(f => f._id, formId);
            return await _formulaireCollection.Find(filter).FirstOrDefaultAsync(cancellationToken);
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

        public async Task<FormulaireObjectDTO> GetFormulaireByIdAsync(ObjectId objectId, CancellationToken cancellationToken)
        {
            
            var filter = Builders<FormulaireObjectDTO>.Filter.Eq(x => x._id, objectId);

            return await _formulaireCollection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<Result<string>> SaveFormAsync(FormulaireObjectSubmitedDTO formulaireSubmited, CancellationToken cancellationToken)
        {
            try
            {
                await _formulairesCollection.InsertOneAsync(formulaireSubmited, null, cancellationToken);
                return Result.Ok("form Submited"); 
            }
            catch (Exception ex)
            {
                // Using the generic error handler to wrap the exception message
                return Result.Fail<string>($"An error occurred while saving the form: {ex.Message}");
            }
        }
        public async Task<List<FormulaireObjectDTO>> GetFormsBySiteWebIdAsync(ObjectId siteWebId, CancellationToken cancellationToken)
        {
            var filter = Builders<FormulaireObjectDTO>.Filter.Eq(f => f.SiteWebId, siteWebId);
            return await _formulaireCollection.Find(filter)
                .Project<FormulaireObjectDTO>(Builders<FormulaireObjectDTO>.Projection
                    .Include(f => f._id) // Ensure `_id` is included
                    .Include(f => f.SiteWebId)
                    .Include(f => f.Formulaire)
                    .Include(f => f.ExcelFileLink)
                     .Include(f => f.CodeBoard)
                    .Include(f => f.Design))
                .ToListAsync(cancellationToken);
        }
        public async Task<Result> UpdateFormulaireAsync(FormulaireObjectDTO formulaire, CancellationToken cancellationToken)
        {
            try
            {
                var filter = Builders<FormulaireObjectDTO>.Filter.Eq(x => x._id, formulaire._id);
                var result = await _formulaireCollection.ReplaceOneAsync(filter, formulaire, cancellationToken: cancellationToken);

                if (result.ModifiedCount == 1)
                {
                    return Result.Ok();
                }
                else
                {
                    return Result.Fail("Formulaire not updated.");
                }
            }
            catch (Exception ex)
            {
                return Result.Fail($"Error updating form: {ex.Message}");
            }
        }
        public async Task<Result> DeleteFormulaireAsync(ObjectId formulaireId, CancellationToken cancellationToken)
        {
            try
            {
                var filter = Builders<FormulaireObjectDTO>.Filter.Eq(x => x._id, formulaireId);
                var deleteResult = await _formulaireCollection.DeleteOneAsync(filter, cancellationToken);

                if (deleteResult.DeletedCount == 1)
                {
                    return Result.Ok();
                }
                else
                {
                    return Result.Fail("Formulaire not deleted.");
                }
            }
            catch (Exception ex)
            {
                return Result.Fail($"Error deleting form: {ex.Message}");
            }
        }
    }
}
