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

        public async Task<FormulaireObjectDTO> GetFormAsync(string siteWebId, string formId, CancellationToken cancellationToken)
        {
            var filter = Builders<FormulaireObjectDTO>.Filter.And(
                Builders<FormulaireObjectDTO>.Filter.Eq(f => f.SiteWebId, new ObjectId(siteWebId)),
                Builders<FormulaireObjectDTO>.Filter.Eq(f => f._id, new ObjectId(formId))
            );

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

        public async Task<FormulaireObjectDTO> GetFormulaireByIdAsync(string formulaireId, CancellationToken cancellationToken)
        {
            var objectId = ObjectId.Parse(formulaireId);
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
    }
}
