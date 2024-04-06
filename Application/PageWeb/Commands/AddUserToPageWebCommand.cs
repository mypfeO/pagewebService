using FluentResults;
using MediatR;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.PageWeb.Commands
{
    public class AddUserToPageWebCommand : IRequest<Result<string>>
    {
        public ObjectId PageWebId { get; set; }
        public ObjectId UserId { get; set; }
    }
}
