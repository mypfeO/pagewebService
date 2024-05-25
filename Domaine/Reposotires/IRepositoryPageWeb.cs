
using Domaine.Entities;
using FluentResults;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Domain.Reposotires
{
    public interface IRepositoryPageWeb
    {
        
        Task<Result> AddPageWebAsync(PageWebDTO pageweb, CancellationToken cancellationToken);
        Task<PageWebDTO> GetPageWebByIdAsync(ObjectId pageWebId, CancellationToken cancellationToken);
        Task<Result> UpdatePageWebAsync(PageWebDTO pageWeb, CancellationToken cancellationToken);
        public Task<List<PageWebDTO>> GetPageWebsByUserId(ObjectId admin, CancellationToken cancellationToken);

    }
}
