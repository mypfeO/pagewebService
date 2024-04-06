﻿using Application.PageWeb.Commands;
using Domain.Reposotires;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.PageWeb.Handlers
{
    public class AddUserToPageWebCommandHandler : IRequestHandler<AddUserToPageWebCommand, Result<string>>
    {
        private readonly IRepositoryPageWeb _repositoryPageWeb;

        public AddUserToPageWebCommandHandler(IRepositoryPageWeb repositoryPageWeb)
        {
            _repositoryPageWeb = repositoryPageWeb;
        }

        public async Task<Result<string>> Handle(AddUserToPageWebCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var pageWeb = await _repositoryPageWeb.GetPageWebByIdAsync(request.PageWebId, cancellationToken);

                if (pageWeb == null)
                {
                    return Result.Fail($"PageWeb with ID {request.PageWebId} not found.");
                }

                
                pageWeb.Users.Add(request.UserId);

                // Save the updated PageWeb object
                await _repositoryPageWeb.UpdatePageWebAsync(pageWeb, cancellationToken);

                return Result.Ok("bien ajouter user");
            }
            catch (Exception ex)
            {
                return Result.Fail($"Error adding user to PageWeb: {ex.Message}");
            }
        }
    }
}
