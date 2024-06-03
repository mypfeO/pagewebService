using Application.Common.Validator;
using Application.formulaire.Commands;
using Application.formulaire.Queries;
using Application.Models;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ServiceWebPage.Controllers
{
    [ApiController]
    [Route("api/Formulaire")]
    public class FormulairesController : ControllerBase
    {
        private readonly IMediator _mediator;
        public FormulairesController(IMediator mediator)
        {
            _mediator = mediator;
          

        }

        [HttpGet("getFormsBySiteWebId/{siteWebId}")]
        [ProducesResponseType(typeof(List<FormulaireObjectModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IDictionary<string, string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetFormsBySiteWebId(string siteWebId, CancellationToken cancellationToken)
        {
            var query = new GetFormsBySiteWebIdQuery { SiteWebId = siteWebId };
            var validator = new GetFormsBySiteWebIdQueryValidator();
            var validationResult = await validator.ValidateAsync(query, cancellationToken);

            if (!validationResult.IsValid)
            {
                var validationErrors = validationResult.Errors.Select(e => e.ErrorMessage);
                return BadRequest(new { Errors = validationErrors });
            }

            var result = await _mediator.Send(query, cancellationToken);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }
            else if (result.Errors.Any(e => e.Message == "No forms found for the provided SiteWebId"))
            {
                return NotFound(new { Errors = result.Errors.Select(e => e.Message) });
            }
            else
            {
                var errorMessages = result.Errors.Select(e => e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new { Errors = errorMessages });
            }
        }
        [HttpPost("createFormulaire")]
        public async Task<IActionResult> CreateFormulaire(FormulaireObjectModel formulaireModel, CancellationToken cancellationToken)
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
                ExcelFileLink = formulaireModel.ExcelFileLink,
                Design = formulaireModel.Design
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
        [HttpPut("updateFormulaire/{id}")]
        [ProducesResponseType(typeof(IDictionary<string, string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IDictionary<string, string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateFormulaire(string id, [FromBody] FormulaireObjectModel formulaireModel, CancellationToken cancellationToken)
        {
            var validator = new FormulaireObjectModelValidator();
            var validationResult = await validator.ValidateAsync(formulaireModel, cancellationToken);
            if (!validationResult.IsValid)
            {
                var validationErrors = validationResult.Errors.Select(e => e.ErrorMessage);
                return BadRequest(new { Errors = validationErrors });
            }

            var updateFormulaireCommand = new UpdateFormulaireCommand
            {
                Id = id,
                SiteWebId = formulaireModel.SiteWebId,
                Formulaire = formulaireModel.Formulaire,
                ExcelFileLink = formulaireModel.ExcelFileLink,
                ProductImages = formulaireModel.Design.ProductImages, // Base64 strings
                Logo = formulaireModel.Design.Logo, // Base64 string
                BackgroundColor = formulaireModel.Design.BackgroundColor
            };

            var result = await _mediator.Send(updateFormulaireCommand, cancellationToken);

            if (result.IsSuccess)
            {
                var formUrl = $"http://localhost:5173/forms/{formulaireModel.SiteWebId}/{id}";
                return Ok(new { Message = "Form updated successfully.", FormUrl = formUrl });
            }
            else if (result.Errors.Any(e => e.Message == "Formulaire not found."))
            {
                return NotFound(new { Errors = result.Errors.Select(e => e.Message) });
            }
            else
            {
                var errorMessages = result.Errors.Select(e => e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new { Errors = errorMessages });
            }
        }
        [HttpDelete("deleteFormulaire/{id}")]
        [ProducesResponseType(typeof(IDictionary<string, string>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteFormulaire(string id, CancellationToken cancellationToken)
        {
            var deleteFormulaireCommand = new DeleteFormulaireCommand { Id = id };
            var result = await _mediator.Send(deleteFormulaireCommand, cancellationToken);

            if (result.IsSuccess)
            {
                return Ok(new { Message = "Formulaire deleted successfully." });
            }
            else if (result.Errors.Any(e => e.Message == "Formulaire not found."))
            {
                return NotFound(new { Errors = result.Errors.Select(e => e.Message) });
            }
            else
            {
                var errorMessages = result.Errors.Select(e => e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new { Errors = errorMessages });
            }
        }
    }
}
