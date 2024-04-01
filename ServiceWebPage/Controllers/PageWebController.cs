using Application.Common.Validator;
using Application.Eroors;
using Application.formulaire.Commands;
using Application.formulaire.Queries;
using Application.Models;
using Application.PageWeb.Commands;
using AutoMapper;
using Domaine.Entities;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using MongoDB.Bson;

namespace ServiceWebPage.Controllers
{
    [ApiController]
    [Route("api/PageWeb")]
    public class PageWebController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public PageWebController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] PageWebModel model, CancellationToken cancellationToken)
        {
                var result = await _mediator.Send(new PageWebCreateCommand { Name = model.Name, users = model.Users }, cancellationToken);

                if (result.IsSuccess)
                {
                    return Ok(new { Token = result.Value });
                }
                else
                {
                    return BadRequest(result.Errors);
                }
          
        }
        [HttpPost("createFormulaire")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> CreateFormulaire([FromBody] FormulaireObjectModel formulaireModel, CancellationToken cancellationToken)
        {
            var validationResult = await new FormulaireObjectModelValidator().ValidateAsync(formulaireModel);
            if (validationResult.IsValid)
            {
                var createFormulaireCommand = new CreateFormulaireCommand
                {
                    SiteWebId = formulaireModel.SiteWebId,
                    Formulaire = formulaireModel.Formulaire
                };

                var result = await _mediator.Send(createFormulaireCommand, cancellationToken);

                if (result.IsSuccess)
                {
                    return Ok("Le formulaire a été créé avec succès.");
                }
                else
                {
                    // Gérer les erreurs spécifiques ici
                    if (result.Errors.Any(error => error is FluentValidation.ValidationException))
                    {
                        var validationErrors = result.Errors
                            .Where(error => error is FluentValidation.ValidationException)
                            .SelectMany(error => ((FluentValidation.ValidationException)error).Errors)
                            .Select(error => error.ErrorMessage);

                        return BadRequest("Une erreur s'est produite lors de la création du formulaire. " + string.Join(", ", validationErrors));
                    }
                    else
                    {
                        // Gérer les autres erreurs génériques ici
                        var errorResult = EroorsHandler.HandleGenericError("Une erreur s'est produite lors de la création du formulaire.");
                        return BadRequest(new { Message = errorResult.Errors.FirstOrDefault()?.Message });
                    }
                }
            }
            else
            {
                var validationErrors = validationResult.Errors.Select(error => error.ErrorMessage);
                return BadRequest("Erreurs de validation : " + string.Join(", ", validationErrors));
            }
        }

        [HttpGet("getFormsBySiteId")]
        public async Task<IActionResult> GetFormsBySiteId(string siteWebId, CancellationToken cancellationToken)
        {
            var query = new GetFormsBySiteIDQuery { Id = siteWebId };

            var result = await _mediator.Send(query, cancellationToken);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }
            else
            {
                return BadRequest(new { Errors = result.Errors });
            }
        }

        [HttpPost("pages/{pageWebId}/adduser")]
        public async Task<IActionResult> AddUserToPageWeb(string pageWebId, [FromBody] UserModel request, CancellationToken cancellationToken)
        {
            var command = new AddUserToPageWebCommand
            {
                PageWebId = new ObjectId(pageWebId),
                UserId = new ObjectId(request.Id)
            };

            var result = await _mediator.Send(command);

            if (result.IsFailed)
                return BadRequest(result.Errors.FirstOrDefault()?.Message);

            return Ok();
        }



    }

}
