using Application.Models;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Validator
{
    public class SubmitFormModelValidator : AbstractValidator<SubmitFormModel>
    {
        public SubmitFormModelValidator()
        {
            RuleFor(x => x.ExcelFileLink)
          .NotEmpty().WithMessage("Le lien du fichier Excel ne peut pas être vide.");

            RuleForEach(x => x.Body)
                .SetValidator(new FormFieldModelValidator());
        }
    }

    public class FormFieldModelValidator : AbstractValidator<FormFieldModel>
    {
        public FormFieldModelValidator()
        {
            RuleFor(x => x.Titre)
                .NotEmpty().WithMessage("Title cannot be empty.");

            RuleFor(x => x.Type)
                .NotEmpty().WithMessage("Type cannot be empty.")
                .Must(type => new List<string> { "image", "video", "text" }.Contains(type.ToLower()))
                .WithMessage("Invalid type specified.");

            RuleFor(x => x.RespenseText)
                .NotEmpty().When(x => x.Type.ToLower() == "text").WithMessage("Response text cannot be empty for text type.");

            RuleFor(x => x.RespenseBase64)
                .NotEmpty().When(x => x.Type.ToLower() == "socle image" || x.Type.ToLower() == "socle video").WithMessage("Response base64 string cannot be empty for image or video type.");
        }
    }


}
