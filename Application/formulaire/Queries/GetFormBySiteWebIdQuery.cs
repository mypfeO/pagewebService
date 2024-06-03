using Application.Models;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.formulaire.Queries
{
    public class GetFormsBySiteWebIdQuery : IRequest<Result<List<FormulaireObjectModel>>>
    {
        public string SiteWebId { get; set; }
    }
}
