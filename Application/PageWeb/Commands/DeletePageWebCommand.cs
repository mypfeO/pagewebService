using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.PageWeb.Commands
{
    public class DeletePageWebCommand : IRequest<Result>
    {
        public string Id { get; set; }
    }
}
