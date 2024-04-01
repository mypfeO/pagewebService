using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domaine.Entities;
using FluentValidation;
using FluentValidation.Validators;
using FluentValidation;
using Application.Models;

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

            RuleFor(x => x.Footer)
                .NotNull().WithMessage("Le champ Footer ne peut pas être null.")
                .SetValidator(new FooterDTOValidator());
        }
    }

    // Les autres classes de validation restent inchangées
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

            RuleFor(x => x.ImageLink)
                .NotEmpty().WithMessage("Le lien de l'image ne peut pas être vide.")
                .MaximumLength(255).WithMessage("Le lien de l'image ne peut pas dépasser 255 caractères.");
        }
    }

    public class FooterDTOValidator : AbstractValidator<FooterDTO>
    {
        public FooterDTOValidator()
        {
            RuleFor(x => x.Titre)
                .NotEmpty().WithMessage("Le titre ne peut pas être vide.")
                .MaximumLength(100).WithMessage("Le titre ne peut pas dépasser 100 caractères.");
        }
    }
}
