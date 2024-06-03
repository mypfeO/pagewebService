using Amazon.Runtime.Internal;
using Application.Eroors;
using Application.formulaire.Commands;
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
using System.Text;
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

                var formulaireDTO = _mapper.Map<FormulaireDTO>(request.Formulaire);

                // Update Google Sheets file with the titles from the Body field
                var spreadsheetId = request.ExcelFileLink; // Assuming this is the correct Spreadsheet ID
                var range = "Feuille 1!A1:Z1"; // Writing headers in the first row
                await UpdateGoogleSheetHeaders(spreadsheetId, range, request.Formulaire.Body.Select(b => b.Titre).ToList(), cancellationToken);

                // Upload images to Cloudinary
                var designDTO = new DesignDTO
                {
                    BackgroundColor = request.BackgroundColor,
                    Logo = await _cloudinaryService.UploadBase64ImageAsync(request.Logo)
                };

                var productImages = new List<string>();
                foreach (var image in request.ProductImages)
                {
                    var imageUrl = await _cloudinaryService.UploadBase64ImageAsync(image);
                    productImages.Add(imageUrl);
                }
                designDTO.ProductImages = productImages;

                existingFormulaire.Formulaire = formulaireDTO;
                existingFormulaire.ExcelFileLink = request.ExcelFileLink;
                existingFormulaire.Design = designDTO;

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
