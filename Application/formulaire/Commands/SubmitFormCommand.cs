using Application.Models;
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
    public class SubmitFormCommand : IRequest
    {
        public FormulaireObjectModel Form { get; set; }
    }
}
