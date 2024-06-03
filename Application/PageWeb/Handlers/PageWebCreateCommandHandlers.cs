using Application.PageWeb.Commands;
using Domain.Reposotires;
using Domaine.Entities;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.PageWeb.Handlers
{
    public class PageWebCreateCommandHandlers : IRequestHandler<PageWebCreateCommand, Result<string>>
    {
        private readonly IRepositoryPageWeb _repositoryPageWeb;
        public PageWebCreateCommandHandlers(IRepositoryPageWeb repositoryPageWeb)
        {
            _repositoryPageWeb = repositoryPageWeb;
        }

        public async Task<Result<string>> Handle(PageWebCreateCommand request, CancellationToken cancellationToken)
        {
            if (!ObjectId.TryParse(request.Admin, out ObjectId objectAdmin))
            {
                return Result.Fail<string>("Invalid admin ID format.");
            }

            try
            {
                var newPageWeb = new PageWebDTO
                {
                    Name = request.Name,
                    Admin = objectAdmin,
                    Theme = request.Theme  // Add this line for Theme
                };

                var result = await _repositoryPageWeb.AddPageWebAsync(newPageWeb, cancellationToken);

                if (result.IsSuccess)
                {
                    return Result.Ok("La page web a été créée avec succès.");
                }
                else
                {
                    return Result.Fail<string>($"Erreur lors de la création de la page web: {result.Errors.FirstOrDefault()?.Message}");
                }
            }
            catch (Exception ex)
            {
                return Result.Fail<string>($"Erreur inattendue : {ex.Message}");
            }
        }
    }

}
