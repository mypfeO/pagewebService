using Application.Common.Validator;
using Application.Eroors;
using Application.formulaire.Commands;
using Application.formulaire.Queries;
using Application.Models;
using Application.PageWeb.Commands;
using AutoMapper;
using Domaine.Entities;
using FluentResults;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

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

        [HttpPost("createWebPage")]
        [ProducesResponseType(typeof(IDictionary<string, string>), StatusCodes.Status200OK)] 
        [ProducesResponseType(typeof(IDictionary<string, string>), StatusCodes.Status400BadRequest)] 
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] 
        public async Task<IActionResult> CreateWebPage([FromBody] PageWebModel model, CancellationToken cancellationToken)
        {
            var validationResult =  await new PageWebModelValidator().ValidateAsync(model, cancellationToken);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(new { Errors = errors });
            }

            var result = await _mediator.Send(new PageWebCreateCommand { Name = model.Name, users = model.Users }, cancellationToken);

            if (result.IsSuccess)
            {
                return Ok(new { Message = result.Value});
            }
            else
            {
                var errorMessages = result.Errors.Select(e => e.Message);
                return BadRequest(new { Errors = errorMessages });
            }
        }
        [HttpPost("createFormulaire")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)] 
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)] 
        public async Task<IActionResult> CreateFormulaire([FromBody] FormulaireObjectModel formulaireModel, CancellationToken cancellationToken)
        {
            var validator = new FormulaireObjectModelValidator();
            var validationResult = await validator.ValidateAsync(formulaireModel, cancellationToken);
            if (!validationResult.IsValid)
            {

                var validationErrors = validationResult.Errors.Select(e => e.ErrorMessage);
                return BadRequest(new { Errors = validationErrors });
            }

            var createFormulaireCommand = new CreateFormulaireCommand
            {
                SiteWebId = formulaireModel.SiteWebId,
                Formulaire = formulaireModel.Formulaire,
                ExcelFileLink = formulaireModel.ExcelFileLink
            };

            var result = await _mediator.Send(createFormulaireCommand, cancellationToken);

            if (result.IsSuccess)
            {
                 var formUrl = $"{Request.Scheme}://{Request.Host}/forms/{formulaireModel.SiteWebId}/{result.Value}";
                return Ok(new { Message = "Form created successfully.", FormUrl = formUrl });
            }
            else
            {
               
                var errorMessages = result.Errors.Select(e => e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new { Errors = errorMessages });
            }
        }
        [HttpGet("{siteWebId}/{formId}")]
        [ProducesResponseType(typeof(FormulaireObjectModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetForm(string siteWebId, string formId, CancellationToken cancellationToken)
        {
            var query = new GetFormQuery { SiteWebId = siteWebId, FormId = formId };

            // La validation peut être faite ici si nécessaire, ou assurée par des Data Annotations sur GetFormQuery

            var result = await _mediator.Send(query, cancellationToken);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }
            else
            {
                if (result.Errors.Any(e => e.Message.Contains("not found")))
                {
                    return NotFound("The requested form was not found.");
                }
                else
                {
                    // Log des erreurs pour un diagnostic plus profond peut être fait ici
                    return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
                }
            }
        }

        [HttpPost("submit")]
        public async Task<IActionResult> Submit([FromBody] FormulaireObjectModel formModel)
        {
            var command = new SubmitFormCommand { Form = formModel };
            await _mediator.Send(command);
            return Ok("Form submitted successfully.");
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

            return Ok(new {Message = result.Value});
        }
       


    }

}
