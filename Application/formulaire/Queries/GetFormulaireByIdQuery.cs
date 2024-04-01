using Domaine.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.formulaire.Queries
{
    public class GetFormulaireByIdQuery : IRequest<FormulaireObjectDTO>
    {
        public string? FormulaireId { get; set; }
    }

}
