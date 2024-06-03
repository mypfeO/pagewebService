using Amazon.Runtime.Internal;
using Application.Eroors;
using Application.formulaire.Commands;
using Application.Models;
using AutoMapper;
using Domaine.Entities;
using Domaine.Reposotires;
using FluentResults;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Sheets.v4;
using Infrastructure.Cloudery;
using MediatR;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.formulaire.Handlers.Commands
{
    public class CreateFormulaireCommandHandler : IRequestHandler<CreateFormulaireCommand, Result<string>>
    {
        private readonly IRepositoryFormulaire _repositoryFormulaire;
        private readonly IMapper _mapper;
        private readonly CloudinaryService _cloudinaryService;
        private readonly GoogleApiSettings _googleApiSettings;
        private SheetsService _sheetsService;

        public CreateFormulaireCommandHandler(
            IRepositoryFormulaire repositoryFormulaire,
            IMapper mapper,
            CloudinaryService cloudinaryService,
            IOptions<GoogleApiSettings> googleApiSettings)
        {
            _repositoryFormulaire = repositoryFormulaire;
            _mapper = mapper;
            _cloudinaryService = cloudinaryService;
            _googleApiSettings = googleApiSettings.Value;
            InitializeSheetsService();
        }

        private void InitializeSheetsService()
        {
            GoogleCredential credential = GoogleCredential.FromFile(_googleApiSettings.CredentialsFilePath)
                .CreateScoped(SheetsService.Scope.Spreadsheets);
            if (credential == null)
            {
                throw new InvalidOperationException("Failed to create Google credentials.");
            }

            _sheetsService = new SheetsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "Google Sheets API Integration",
            });
        }

        public async Task<Result<string>> Handle(CreateFormulaireCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (!ObjectId.TryParse(request.SiteWebId, out ObjectId siteWebObjectId))
                {
                    return EroorsHandler.HandleGenericError<string>("Invalid SiteWebId format.");
                }

                // Manually fill the Formulaire object
                var formulaireDTO = new FormulaireDTO
                {
                    Head = _mapper.Map<HeadDTO>(request.Formulaire.Head),
                    Body = await MapBodyItems(request.Formulaire.Body),
                    Footer = _mapper.Map<FooterDTO>(request.Formulaire.Footer)
                };

                var headersToWrite = formulaireDTO.Body
                    .Where(b => b.Type != "image" && b.Type != "video")
                    .Select(b => b.Titre)
                    .ToList();

                // Update Google Sheets file with the titles from the Body field
                var spreadsheetId = request.ExcelFileLink; // Assuming this is the correct Spreadsheet ID
                var range = "Feuille 1!A1:Z1"; // Writing headers in the first row
                await WriteHeadersToGoogleSheet(spreadsheetId, range, headersToWrite, cancellationToken);

                // Upload images in the design to Cloudinary
                var designDTO = new DesignDTO
                {
                    BackgroundColor = request.Design.BackgroundColor,
                    Logo = await _cloudinaryService.UploadBase64ImageAsync(request.Design.Logo),
                    ProductImages = (await Task.WhenAll(request.Design.ProductImages.Select(image => _cloudinaryService.UploadBase64ImageAsync(image)))).ToList()
                };

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

        private async Task<List<BodyItemDTO>> MapBodyItems(List<BodyItem> bodyItems)
        {
            var bodyItemDTOs = new List<BodyItemDTO>();

            foreach (var bodyItem in bodyItems)
            {
                var bodyItemDTO = new BodyItemDTO
                {
                    Titre = bodyItem.Titre,
                    Type = bodyItem.Type,
                    Required = bodyItem.Required
                };

                switch (bodyItem.Type)
                {
                    case "image":
                        if (!string.IsNullOrEmpty(bodyItem.RespenseText))
                        {
                            var imageUrl = await _cloudinaryService.UploadBase64ImageAsync(bodyItem.RespenseText);
                            bodyItemDTO.RespenseText = imageUrl;
                        }
                        break;
                    case "video":
                        if (!string.IsNullOrEmpty(bodyItem.RespenseText))
                        {
                            var videoUrl = await _cloudinaryService.UploadVideoFromBase64Async(bodyItem.RespenseText);
                            bodyItemDTO.RespenseText = videoUrl;
                        }
                        break;
                    case "Text":
                    case "Date":
                        bodyItemDTO.RespenseText = bodyItem.RespenseText;
                        break;
                }

                bodyItemDTOs.Add(bodyItemDTO);
            }

            return bodyItemDTOs;
        }

        private async Task WriteHeadersToGoogleSheet(string spreadsheetId, string range, List<string> headers, CancellationToken cancellationToken)
        {
            var valueRange = new ValueRange
            {
                Values = new List<IList<object>> { headers.Cast<object>().ToList() }
            };

            var updateRequest = _sheetsService.Spreadsheets.Values.Update(valueRange, spreadsheetId, range);
            updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
            await updateRequest.ExecuteAsync(cancellationToken);
        }
    }
}
