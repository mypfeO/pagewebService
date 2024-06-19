using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domaine.Entities;
using FluentValidation;
using FluentValidation.Validators;
using Application.Models;
using Application.formulaire.Queries;
using Application.formulaire.Commands;
using MongoDB.Bson;

namespace Application.Common.Validator
{
    public class FormulaireObjectModelValidator : AbstractValidator<FormulaireObjectModel>
    {
        public FormulaireObjectModelValidator()
        {
            RuleFor(x => x.SiteWebId)
                .NotEmpty().WithMessage("SiteWebId ne peut pas être vide.")
                .NotNull().WithMessage("SiteWebId ne peut pas être null.");

            RuleFor(x => x.Formulaire)
                .NotNull().WithMessage("Formulaire ne peut pas être null.");

            RuleFor(x => x.ExcelFileLink)
          .NotEmpty().WithMessage("Le lien du fichier Excel ne peut pas être vide.")
        //  .Must(Link => Uri.IsWellFormedUriString(Link, UriKind.Absolute)).WithMessage("Le lien du fichier Excel n'est pas un URI valide.");
        .Length(2, 50).WithMessage("link must be between 2 and 50 characters.");
            RuleFor(x => x.Design)
           .NotNull().WithMessage("Design cannot be null.")
           .SetValidator(new DesignValidator());
            RuleFor(x => x.CodeBoard) // Add validation for the new field
          .NotEmpty().WithMessage("CodeBoard cannot be empty.")
          .MaximumLength(50).WithMessage("CodeBoard must be 50 characters or less.");

        }
    }

    public class DesignValidator : AbstractValidator<DesignSummary>
    {
        public DesignValidator()
        {
            RuleFor(x => x.BackgroundColor)
                .NotEmpty().WithMessage("Background color cannot be empty.")
                .Matches("^#(?:[0-9a-fA-F]{3}){1,2}$").WithMessage("Invalid background color format.");

            RuleFor(x => x.Logo)
                .NotNull().WithMessage("Logo cannot be null.");

            RuleFor(x => x.ProductImages)
                .NotEmpty().WithMessage("At least one ProductImage is required.");
        }
    }

    public class FormulaireDTOValidator : AbstractValidator<FormulaireDTO>
    {
        public FormulaireDTOValidator()
        {
            RuleFor(x => x.Head)
                .NotNull().WithMessage("Le champ Head ne peut pas être null.")
                .SetValidator(new HeadDTOValidator());

            RuleFor(x => x.Body)
                .NotEmpty().WithMessage("La liste Body ne peut pas être vide.")
                .ForEach(bodyItem => bodyItem.SetValidator(new BodyItemDTOValidator()));

            // Validate Footer's Items directly
            RuleFor(x => x.Footer)
                .NotNull().WithMessage("Le champ Footer ne peut pas être null.");

            RuleForEach(x => x.Footer)
                .SetValidator(new FooterItemDTOValidator()); // Validate each FooterItemDTO
        }
    }

    public class HeadDTOValidator : AbstractValidator<HeadDTO>
    {
        public HeadDTOValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Le titre ne peut pas être vide.")
                .MaximumLength(100).WithMessage("Le titre ne peut pas dépasser 100 caractères.");
        }
    }

    public class BodyItemDTOValidator : AbstractValidator<BodyItemDTO>
    {
        public BodyItemDTOValidator()
        {
            RuleFor(x => x.Titre)
                .NotEmpty().WithMessage("Le titre ne peut pas être vide.")
                .MaximumLength(100).WithMessage("Le titre ne peut pas dépasser 100 caractères.");

            RuleFor(x => x.Type)
                .NotEmpty().WithMessage("Le type ne peut pas être vide.");

            // Additional validation based on the type
            When(x => x.Type == "image" || x.Type == "video", () =>
            {
                RuleFor(x => x.RespenseText)
                    .NotEmpty().WithMessage("Le lien de la réponse ne peut pas être vide.");
            });
        }
    }

    public class FooterItemDTOValidator : AbstractValidator<FooterItemDTO>
    {
        public FooterItemDTOValidator()
        {
            RuleFor(x => x.Titre)
                .NotEmpty().WithMessage("Le titre ne peut pas être vide.")
                .MaximumLength(100).WithMessage("Le titre ne peut pas dépasser 100 caractères.");

            RuleFor(x => x.LinkNextForm)
                .NotEmpty().WithMessage("Le lien ne peut pas être vide.")
                .MaximumLength(200).WithMessage("Le lien ne peut pas dépasser 200 caractères.");
        }
    }

    public class GetFormQueryValidator : AbstractValidator<GetFormulaireQuery>
    {
        public GetFormQueryValidator()
        {
            RuleFor(query => query.SiteWebId)
                .NotEmpty().WithMessage("SiteWebId must be provided.");

            RuleFor(query => query.FormId)
                .NotEmpty().WithMessage("FormId must be provided.");
        }
    }
}