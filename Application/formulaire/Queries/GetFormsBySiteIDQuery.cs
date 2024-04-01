using Amazon.Runtime.Internal;
using Application.Common.Mappings;
using Application.Models;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.formulaire.Queries
{
    public class GetFormsBySiteIDQuery : IRequest<Result<FormulaireSummary>>
    {
       public string? Id { get; set; }
    }
}
