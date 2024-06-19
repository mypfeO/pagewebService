using Amazon.Runtime.Internal;
using Application.Eroors;
using Application.formulaire.Commands;
using AutoMapper;
using Domaine.Entities;
using Domaine.Reposotires;
using FluentResults;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
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
    public class UpdateFormulaireCommandHandler : IRequestHandler<UpdateFormulaireCommand, Result<string>>
    {
        private readonly IRepositoryFormulaire _repositoryFormulaire;
        private readonly IMapper _mapper;
        private readonly CloudinaryService _cloudinaryService;
        private readonly GoogleApiSettings _googleApiSettings;
        private SheetsService _sheetsService;

        public UpdateFormulaireCommandHandler(
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

        public async Task<Result<string>> Handle(UpdateFormulaireCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (!ObjectId.TryParse(request.Id, out ObjectId formulaireId) ||
                    !ObjectId.TryParse(request.SiteWebId, out ObjectId siteWebObjectId))
                {
                    return EroorsHandler.HandleGenericError<string>("Invalid ID format.");
                }

                var existingFormulaire = await _repositoryFormulaire.GetFormulaireByIdAsync(formulaireId, cancellationToken);
                if (existingFormulaire == null)
                {
                    return EroorsHandler.HandleGenericError<string>("Formulaire not found.");
                }

                // Handle respenseText for socle images and socle videos in Formulaire Body
                foreach (var bodyItem in request.Formulaire.Body)
                {
                    if ((bodyItem.Type == "image" || bodyItem.Type == "video") && !string.IsNullOrEmpty(bodyItem.RespenseText))
                    {
                        bodyItem.RespenseText = await ProcessContent(bodyItem.RespenseText);
                    }
                }

                // Check and upload Logo
                var logoUrl = await ProcessContent(request.Logo);

                // Check and upload ProductImages
                var productImagesUrls = new List<string>();
                foreach (var image in request.ProductImages)
                {
                    var imageUrl = await ProcessContent(image);
                    productImagesUrls.Add(imageUrl);
                }

                var designDTO = new DesignDTO
                {
                    BackgroundColor = request.BackgroundColor,
                    Logo = logoUrl,
                    ProductImages = productImagesUrls
                };

                var formulaireDTO = _mapper.Map<FormulaireDTO>(request.Formulaire);
                existingFormulaire.Formulaire = formulaireDTO;
                existingFormulaire.ExcelFileLink = request.ExcelFileLink;
                existingFormulaire.CodeBoard = request.CodeBoard;
                existingFormulaire.Design = designDTO;

                // Update Google Sheets with new headers for socle image or socle video
                var spreadsheetId = request.ExcelFileLink; // Assuming this is the correct Spreadsheet ID
                var range = "Feuille 1!A1:Z1"; // Writing headers in the first row

                // Filter headers for socle image and socle video
                var filteredHeaders = request.Formulaire.Body
                    .Where(b => b.Type == "socle image" || b.Type == "socle video" ||b.Type=="text")
                    .Select(b => b.Titre)
                    .ToList();

                await UpdateGoogleSheetHeaders(spreadsheetId, range, filteredHeaders, cancellationToken);

                var result = await _repositoryFormulaire.UpdateFormulaireAsync(existingFormulaire, cancellationToken);

                if (result.IsSuccess)
                {
                    return Result.Ok(existingFormulaire._id.ToString());
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

        private async Task<string> ProcessContent(string content)
        {
            if (IsCloudinaryUrl(content))
            {
                return content;
            }

            return await _cloudinaryService.UploadBase64ImageAsync(content);
        }

        private bool IsCloudinaryUrl(string input)
        {
            return input.StartsWith("http://res.cloudinary.com") ||
                   input.StartsWith("https://res.cloudinary.com");
        }

        private async Task UpdateGoogleSheetHeaders(string spreadsheetId, string range, List<string> newHeaders, CancellationToken cancellationToken)
        {
            // Clear existing headers in the specified range
            var clearRequest = new ClearValuesRequest();
            var clearResponse = await _sheetsService.Spreadsheets.Values.Clear(clearRequest, spreadsheetId, range).ExecuteAsync(cancellationToken);

            // Write new headers to the specified range
            var valueRange = new ValueRange
            {
                Values = new List<IList<object>> { newHeaders.Cast<object>().ToList() }
            };

            var updateRequest = _sheetsService.Spreadsheets.Values.Update(valueRange, spreadsheetId, range);
            updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
            await updateRequest.ExecuteAsync(cancellationToken);
        }
    }
}
