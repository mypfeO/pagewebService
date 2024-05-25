using Application.Common.Validator;
using Application.Eroors;
using Application.formulaire.Commands;
using Application.formulaire.Queries;
using Application.Models;
using Application.PageWeb.Commands;
using Application.PageWeb.Querys;
using AutoMapper;
using Domaine.Entities;
using FluentResults;
using FluentValidation.Results;
using Infrastructure.Cloudery;
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
        private readonly ILogger<GetWebPagesByUserIdRequest> _logger;
        public PageWebController(IMediator mediator, IMapper mapper, ILogger<GetWebPagesByUserIdRequest> logger)
        {
            _mediator = mediator;
            _mapper = mapper;
            _logger= logger;

        }
        [HttpGet("by-webid/{siteWebId}")]
        public async Task<IActionResult> GetFormBySiteWebId(string siteWebId)
        {
            var query = new GetFormsBySiteWebIdQuery { SiteWebId = siteWebId };
            var result = await _mediator.Send(query);
            if (result == null)
                return NotFound();

            return Ok(result);
        }
        [HttpGet("by-user/{Admin}")]
        public async Task<IActionResult> GetPageWebsByUserId([FromRoute] GetWebPagesByUserIdRequest request)
        {
            _logger.LogInformation("Received UserId: {UserId}", request.Admin);  // Ensure your controller has a logger injected if it doesn't already.

            if (!ObjectId.TryParse(request.Admin, out ObjectId userObjectId))
            {
                _logger.LogWarning("Failed to parse ObjectId from UserId: {UserId}", request.Admin);
                return BadRequest("Invalid user ID format.");
            }

            var query = new GetPageWebsByUserIdQuery { Admin = userObjectId };
            try
            {
                var pageWebs = await _mediator.Send(query);
                return Ok(pageWebs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching page webs");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred: " + ex.Message);
            }
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

            var result = await _mediator.Send(new PageWebCreateCommand { Name = model.Name,Admin=model.Admin, users = model.Users }, cancellationToken);

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
        public async Task<IActionResult> CreateFormulaire([FromForm] FormulaireObjectModel formulaireModel, CancellationToken cancellationToken)
        {
            Console.WriteLine("Received FormulaireObjectModel: ");
            Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(formulaireModel));

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
                ExcelFileLink = formulaireModel.ExcelFileLink,
                ProductImages = formulaireModel.Design.ProductImages,
                Logo = formulaireModel.Design.Logo,
                BackgroundColor = formulaireModel.Design.BackgroundColor
            };

            var result = await _mediator.Send(createFormulaireCommand, cancellationToken);

            if (result.IsSuccess)
            {
                var formUrl = $"http://localhost:5173/forms/{formulaireModel.SiteWebId}/{result.Value}";
                return Ok(new { Message = "Form created successfully.", FormUrl = formUrl });
            }
            else
            {
                var errorMessages = result.Errors.Select(e => e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new { Errors = errorMessages });
            }
        }
        [HttpGet("{siteWebId}/{formId}")]
        [ProducesResponseType(typeof(GetFormsById), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetFormulaire(string siteWebId, string formId, CancellationToken cancellationToken)
        {
            var query = new GetFormulaireQuery { SiteWebId = siteWebId, FormId = formId };
            var result = await _mediator.Send(query, cancellationToken);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }
            else
            {
                var errorMessages = result.Errors.Select(e => e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new { Errors = errorMessages });
            }
        }

        [HttpPost("submit")]
        public async Task<IActionResult> Submit([FromForm] SubmitFormModel model)
        {
           

            var command = new SubmitFormCommand { Form = model };
            await _mediator.Send(command);
            return Ok(new { Message = "Form submitted successfully." });
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
