using Application.PageWeb.Commands;
using Domain.Reposotires;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Eroors;
using Application.Models;
using MongoDB.Bson;

namespace Application.PageWeb.Handlers
{
    public class UpdatePageWebCommandHandler : IRequestHandler<UpdatePageWebCommand, Result<string>>
    {
        private readonly IRepositoryPageWeb _repositoryPageWeb;

        public UpdatePageWebCommandHandler(IRepositoryPageWeb repositoryPageWeb)
        {
            _repositoryPageWeb = repositoryPageWeb;
        }

        public async Task<Result<string>> Handle(UpdatePageWebCommand request, CancellationToken cancellationToken)
        {
            if (!ObjectId.TryParse(request.Admin, out ObjectId objectAdmin) || !ObjectId.TryParse(request.Id, out ObjectId pageWebId))
            {
                return Result.Fail<string>("Invalid ID format.");
            }

            try
            {
                var pageWeb = await _repositoryPageWeb.GetPageWebByIdAsync(pageWebId, cancellationToken);
                if (pageWeb == null)
                {
                    return Result.Fail<string>("PageWeb not found.");
                }

                pageWeb.Name = request.Name;
                pageWeb.Admin = objectAdmin;
                pageWeb.Theme = request.Theme;

                var result = await _repositoryPageWeb.UpdatePageWebAsync(pageWeb, cancellationToken);

                if (result.IsSuccess)
                {
                    return Result.Ok("PageWeb updated successfully.");
                }
                else
                {
                    return Result.Fail<string>($"Error updating PageWeb: {result.Errors.FirstOrDefault()?.Message}");
                }
            }
            catch (Exception ex)
            {
                return Result.Fail<string>($"Unexpected error: {ex.Message}");
            }
        }
    }

}
