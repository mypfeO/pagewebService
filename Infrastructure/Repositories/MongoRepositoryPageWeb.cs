using Amazon.Runtime.Internal;
using Domain.Reposotires;
using Domaine.Entities;
using FluentResults;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class MongoRepositoryPageWeb : IRepositoryPageWeb
    {
        private readonly IMongoCollection<PageWebDTO> _pageWebCollection;
        public MongoRepositoryPageWeb(IMongoDatabase database)
        {
            _pageWebCollection = database.GetCollection<PageWebDTO>("pagewebs");
        }
      

        public async Task<Result> AddPageWebAsync(PageWebDTO pageweb, CancellationToken cancellationToken)
        {
            try
            {
                await _pageWebCollection.InsertOneAsync(pageweb, null, cancellationToken);
                return Result.Ok();
            }
            catch (Exception ex)
            {

                return Result.Fail($"Erreur lors de l'ajout de page web : {ex.Message}");
            }
        }
        public async Task<PageWebDTO> GetPageWebByIdAsync(ObjectId pageWebId, CancellationToken cancellationToken)
        {
            var filter = Builders<PageWebDTO>.Filter.Eq(x => x.Id, pageWebId);
            return await _pageWebCollection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<Result> UpdatePageWebAsync(PageWebDTO pageWeb, CancellationToken cancellationToken)
        {
            var filter = Builders<PageWebDTO>.Filter.Eq(x => x.Id, pageWeb.Id);

            // Explicitly specify ReplaceOptions
            var replaceOptions = new ReplaceOptions { IsUpsert = false };

            var result = await _pageWebCollection.ReplaceOneAsync(filter, pageWeb, replaceOptions, cancellationToken);

            if (result.ModifiedCount == 1)
            {
                return Result.Ok();
            }
            else
            {
                return Result.Fail("PageWeb not updated.");
            }
        }

    }
}
