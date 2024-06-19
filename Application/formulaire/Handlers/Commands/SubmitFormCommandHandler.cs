using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Application.formulaire.Commands;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Linq;
using Infrastructure.Cloudery;
using Google;
using Microsoft.AspNetCore.Http;
using Application;
public class SubmitFormCommandHandler : IRequestHandler<SubmitFormCommand, Unit>
{
    private readonly ILogger<SubmitFormCommandHandler> _logger;
    private readonly GoogleApiSettings _googleApiSettings;
    private readonly CloudinaryService _cloudinaryService;
    private SheetsService _sheetsService;

    public SubmitFormCommandHandler(
        ILogger<SubmitFormCommandHandler> logger,
        IOptions<GoogleApiSettings> googleApiSettings,
        CloudinaryService cloudinaryService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _googleApiSettings = googleApiSettings.Value ?? throw new ArgumentNullException(nameof(googleApiSettings));
        _cloudinaryService = cloudinaryService ?? throw new ArgumentNullException(nameof(cloudinaryService));
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

    public async Task<Unit> Handle(SubmitFormCommand command, CancellationToken cancellationToken)
    {
        var spreadsheetId = command.Form.ExcelFileLink; // Ensure this is the correct Spreadsheet ID
        var range = "Feuille 1"; // Define appropriate range

        try
        {
            List<object> values = new List<object>();

            foreach (var bodyItem in command.Form.Body)
            {
                if (bodyItem != null)
                {
                    switch (bodyItem.Type.ToLower())
                    {
                        case "socle image":
                            if (!string.IsNullOrEmpty(bodyItem.RespenseBase64))
                            {
                                string imageUrl = await _cloudinaryService.UploadBase64ImageAsync(bodyItem.RespenseBase64);
                                bodyItem.RespenseText = imageUrl;
                                values.Add(imageUrl);
                            }
                            break;
                        case "socle video":
                            if (!string.IsNullOrEmpty(bodyItem.RespenseBase64))
                            {
                                string videoUrl = await _cloudinaryService.UploadVideoFromBase64Async(bodyItem.RespenseBase64);
                                bodyItem.RespenseText = videoUrl;
                                values.Add(videoUrl);
                            }
                            break;
                        case "text":
                            values.Add(bodyItem.RespenseText);
                            break;
                        default:
                            values.Add(bodyItem.RespenseText);
                            break;
                    }
                }
            }

            var valueRange = new ValueRange
            {
                Values = new List<IList<object>> { values }
            };

            var appendRequest = _sheetsService.Spreadsheets.Values.Append(valueRange, spreadsheetId, range);
            appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
            await appendRequest.ExecuteAsync(cancellationToken);

            _logger.LogInformation("Submission appended successfully.");
            return Unit.Value;
        }
        catch (GoogleApiException ex)
        {
            _logger.LogError("Error while accessing Google Sheets API: {Exception}", ex);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError("An unexpected error occurred: {Exception}", ex);
            throw;
        }
    }
}
