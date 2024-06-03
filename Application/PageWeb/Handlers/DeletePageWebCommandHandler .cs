using Application.Eroors;
using Application.PageWeb.Commands;
using Domain.Reposotires;
using FluentResults;
using MediatR;
using MongoDB.Bson;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.PageWeb.Handlers
{
    public class DeletePageWebCommandHandler : IRequestHandler<DeletePageWebCommand, Result>
    {
        private readonly IRepositoryPageWeb _repositoryPageWeb;

        public DeletePageWebCommandHandler(IRepositoryPageWeb repositoryPageWeb)
        {
            _repositoryPageWeb = repositoryPageWeb;
        }

        public async Task<Result> Handle(DeletePageWebCommand request, CancellationToken cancellationToken)
        {
            if (!ObjectId.TryParse(request.Id, out ObjectId pageWebId))
            {
                return EroorsHandler.HandleGenericError("Invalid PageWeb ID format.");
            }

            try
            {
                var pageWeb = await _repositoryPageWeb.GetPageWebByIdAsync(pageWebId, cancellationToken);

                if (pageWeb == null)
                {
                    return EroorsHandler.HandleGenericError("PageWeb not found.");
                }

                var deleteResult = await _repositoryPageWeb.DeletePageWebAsync(pageWebId, cancellationToken);

                if (deleteResult.IsSuccess)
                {
                    return Result.Ok();
                }
                else
                {
                    return EroorsHandler.HandleGenericError("Failed to delete PageWeb.");
                }
            }
            catch (Exception ex)
            {
                return EroorsHandler.HandleGenericError($"Unexpected error: {ex.Message}");
            }
        }
    }
}
