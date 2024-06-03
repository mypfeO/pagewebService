using Application.Models;
using Domaine.Entities;
using FluentResults;
using MediatR;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.PageWeb.Querys
{
    public class GetPageWebsByUserIdQuery : IRequest<List<SummaryPageWeb>>
    {
        public ObjectId Admin { get; set; }
    }

}
