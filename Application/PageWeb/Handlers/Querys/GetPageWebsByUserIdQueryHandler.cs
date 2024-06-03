using Application.Models;
using Application.PageWeb.Querys;
using Domain.Reposotires;
using Domaine.Entities;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.PageWeb.Handlers.Querys
{
    public class GetPageWebsByUserIdQueryHandler : IRequestHandler<GetPageWebsByUserIdQuery, List<SummaryPageWeb>>
    {
        private readonly IRepositoryPageWeb _repositoryPageWeb;

        public GetPageWebsByUserIdQueryHandler(IRepositoryPageWeb repositoryPageWeb)
        {
            _repositoryPageWeb = repositoryPageWeb;
        }

       public async Task<List<SummaryPageWeb>> Handle(GetPageWebsByUserIdQuery request, CancellationToken cancellationToken)
    {
        var dtos = await _repositoryPageWeb.GetPageWebsByUserId(request.Admin, cancellationToken);
            var models = dtos.Select(dto => new SummaryPageWeb
            {
                Id = dto.Id.ToString(),  // Assuming dto.Id is already a string.
                Name = dto.Name,
                Theme=dto.Theme,
            }).ToList();

        return models;
    }
    }

}
