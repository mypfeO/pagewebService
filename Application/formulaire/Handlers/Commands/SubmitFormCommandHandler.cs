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
        var range = "Feuille 1!A1:Z1"; // Checking headers in the first row

        try
        {
            List<object> headers = new List<object>();
            List<object> values = new List<object>();

            foreach (var bodyItem in command.Form.Body)
            {
                if (bodyItem != null)
                {
                    if (bodyItem.ImageLink && bodyItem.RespenseFile != null)
                    {
                        string imageUrl = await _cloudinaryService.UploadImageAsync(bodyItem.RespenseFile);
                        bodyItem.RespenseText = imageUrl;
                        values.Add(imageUrl);
                    }
                    else
                    {
                        values.Add(bodyItem.RespenseText);
                    }

                    headers.Add(bodyItem.Titre);
                }
            }

            var valueRange = new ValueRange();
            bool headersExist = await CheckHeadersExist(spreadsheetId, range, cancellationToken);

            // Initialize valueRange.Values to an empty list to ensure it's never null
            valueRange.Values = new List<IList<object>>();

            if (!headersExist)
            {
                valueRange.Values.Add(headers);
            }

            valueRange.Values.Add(values);

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

    private async Task<bool> CheckHeadersExist(string spreadsheetId, string range, CancellationToken cancellationToken)
    {
        var getHeaderRequest = _sheetsService.Spreadsheets.Values.Get(spreadsheetId, range);
        var headerResponse = await getHeaderRequest.ExecuteAsync(cancellationToken);
        return headerResponse.Values != null && headerResponse.Values.Count > 0;
    }

    //private ValueRange PrepareValueRange(List<FormField> fields, bool headersExist)
    //{
    //    var valueRange = new ValueRange();
    //    if (!headersExist)
    //    {
    //        var headersRow = fields.Select(f => f.Titre).Cast<object>().ToList();
    //        valueRange.Values = new List<IList<object>> { headersRow };
    //    }

    //    var responseRow = fields.Select(f => f.RespenseText ?? "").Cast<object>().ToList();
    //    valueRange.Values = valueRange.Values ?? new List<IList<object>>();
    //    valueRange.Values.Add(responseRow);

    //    return valueRange;
    //}
}
