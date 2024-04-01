using Application.Models;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.PageWeb.Commands
{
    public class PageWebCreateCommand : IRequest<Result<string>>
    {
        public string Name { get; set; } = string.Empty;
        public List<ObjectId> users { get; set; } = new List<ObjectId>();

    }
}
