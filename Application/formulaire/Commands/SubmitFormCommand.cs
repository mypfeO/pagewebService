using Domaine.Entities;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.formulaire.Commands
{
    public class SubmitFormCommand : IRequest<Result<string>>
    {
        public string ExcelFileLink { get; set; }

        public FormulaireSubmitedDTO Formulaire { get; set; }
    }
}
