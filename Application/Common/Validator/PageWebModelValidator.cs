using Application.Models;
using FluentValidation;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Validator
{
    public class PageWebModelValidator : AbstractValidator<PageWebModel>
    {
        public PageWebModelValidator()
        {


            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .Length(2, 50).WithMessage("Name must be between 2 and 50 characters.");

            RuleFor(x => x.Admin)
                .NotEmpty().WithMessage("Admin is required.")
                .Must(admin => ObjectId.TryParse(admin, out _)).WithMessage("Admin must be a valid ObjectId.");

            RuleFor(x => x.Theme)
                .NotEmpty().WithMessage("Theme is required.")
                .Length(2, 50).WithMessage("Theme must be between 2 and 50 characters.");
        }
    }
    public class UpdatePageWebModelValidator : AbstractValidator<UpdatePageWeb>
    {
        public UpdatePageWebModelValidator()
        {


            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .Length(2, 50).WithMessage("Name must be between 2 and 50 characters.");

            RuleFor(x => x.Admin)
                .NotEmpty().WithMessage("Admin is required.")
                .Must(admin => ObjectId.TryParse(admin, out _)).WithMessage("Admin must be a valid ObjectId.");

            RuleFor(x => x.Theme)
                .NotEmpty().WithMessage("Theme is required.")
                .Length(2, 50).WithMessage("Theme must be between 2 and 50 characters.");
        }
    }

}
