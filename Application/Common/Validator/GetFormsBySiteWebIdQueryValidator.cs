using Application.formulaire.Queries;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Validator
{
    public class GetFormsBySiteWebIdQueryValidator : AbstractValidator<GetFormsBySiteWebIdQuery>
    {
        public GetFormsBySiteWebIdQueryValidator()
        {
            RuleFor(query => query.SiteWebId).NotEmpty().WithMessage("SiteWebId must be provided.");
        }
    }

}
