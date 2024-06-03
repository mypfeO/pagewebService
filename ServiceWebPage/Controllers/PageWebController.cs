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
            var validationResult = await new PageWebModelValidator().ValidateAsync(model, cancellationToken);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(new { Errors = errors });
            }

            var pageWebCreateCommand = new PageWebCreateCommand
            {
                Name = model.Name,
                Admin = model.Admin,
                Theme = model.Theme
            };

            var result = await _mediator.Send(pageWebCreateCommand, cancellationToken);

            if (result.IsSuccess)
            {
                return Ok(new { Message = result.Value });
            }
            else
            {
                var errorMessages = result.Errors.Select(e => e.Message);
                return BadRequest(new { Errors = errorMessages });
            }
        }
        [HttpPut("updateWebPage")]
        [ProducesResponseType(typeof(IDictionary<string, string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IDictionary<string, string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateWebPage([FromBody] UpdatePageWeb model, CancellationToken cancellationToken)
        {
            var validationResult = await new UpdatePageWebModelValidator().ValidateAsync(model, cancellationToken);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(new { Errors = errors });
            }

            var updatePageWebCommand = new UpdatePageWebCommand
            {
                Id = model.Id,
                Name = model.Name,
                Admin = model.Admin,
                Theme = model.Theme
            };

            var result = await _mediator.Send(updatePageWebCommand, cancellationToken);

            if (result.IsSuccess)
            {
                return Ok(new { Message = result.Value });
            }
            else
            {
                var errorMessages = result.Errors.Select(e => e.Message);
                return BadRequest(new { Errors = errorMessages });
            }
        }




        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeletePageWeb(string id, CancellationToken cancellationToken)
        {
            var command = new DeletePageWebCommand { Id = id };
            var result = await _mediator.Send(command, cancellationToken);

            if (result.IsSuccess)
            {
                return Ok(new { Message = "Page web deleted successfully." });
            }
            else if (result.Errors.Any(e => e.Message == "PageWeb not found."))
            {
                return NotFound(new { Errors = result.Errors.Select(e => e.Message) });
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Errors = result.Errors.Select(e => e.Message) });
            }
        }




    }

}
