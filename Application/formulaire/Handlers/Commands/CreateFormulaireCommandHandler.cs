using Application.Eroors;
using Application.formulaire.Commands;
using Application.Models;
using AutoMapper;
using Domaine.Entities;
using Domaine.Reposotires;
using FluentResults;
using Infrastructure.Cloudery;
using MediatR;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.formulaire.Handlers.Commands
{
    public class CreateFormulaireCommandHandler : IRequestHandler<CreateFormulaireCommand, Result<string>>
    {
        private readonly IRepositoryFormulaire _repositoryFormulaire;
        private readonly IMapper _mapper;
        private readonly CloudinaryService _cloudinaryService;

        public CreateFormulaireCommandHandler(IRepositoryFormulaire repositoryFormulaire, IMapper mapper, CloudinaryService cloudinaryService)
        {
            _repositoryFormulaire = repositoryFormulaire;
            _mapper = mapper;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<Result<string>> Handle(CreateFormulaireCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (!ObjectId.TryParse(request.SiteWebId, out ObjectId siteWebObjectId))
                {
                    return EroorsHandler.HandleGenericError<string>("Invalid SiteWebId format.");
                }

                var formulaireDTO = _mapper.Map<FormulaireDTO>(request.Formulaire);

                // Upload images to Cloudinary
                var designDTO = new DesignDTO
                {
                    BackgroundColor = request.BackgroundColor,
                    Logo = await _cloudinaryService.UploadImageAsync(request.Logo)
                };

                var productImages = new List<string>();
                foreach (var image in request.ProductImages)
                {
                    var imageUrl = await _cloudinaryService.UploadImageAsync(image);
                    productImages.Add(imageUrl);
                }
                designDTO.ProductImages = productImages;

                var formulaireModel = new FormulaireObjectDTO
                {
                    SiteWebId = siteWebObjectId,
                    Formulaire = formulaireDTO,
                    ExcelFileLink = request.ExcelFileLink,
                    Design = designDTO
                };

                var result = await _repositoryFormulaire.AddFormulaireAsync(formulaireModel, cancellationToken);

                if (result.IsSuccess)
                {
                    return Result.Ok(formulaireModel._id.ToString());
                }
                else
                {
                    return EroorsHandler.HandleGenericError<string>($"Error: {result.Errors.First().Message}");
                }
            }
            catch (Exception ex)
            {
                return EroorsHandler.HandleGenericError<string>($"Unexpected error: {ex.Message}");
            }
        }
    }
}
