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
using Application;
using Google;

public class SubmitFormCommandHandler : IRequestHandler<SubmitFormCommand, Unit>
{
    private readonly ILogger<SubmitFormCommandHandler> _logger;
    private readonly GoogleApiSettings _googleApiSettings;
    private SheetsService _sheetsService;

    public SubmitFormCommandHandler(ILogger<SubmitFormCommandHandler> logger, IOptions<GoogleApiSettings> googleApiSettings)
    {
        _logger = logger;
        _googleApiSettings = googleApiSettings.Value;
        InitializeSheetsService();
    }

    private void InitializeSheetsService()
    {
        GoogleCredential credential = GoogleCredential.FromFile(_googleApiSettings.CredentialsFilePath)
            .CreateScoped(SheetsService.Scope.Spreadsheets);
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
            var getHeaderRequest = _sheetsService.Spreadsheets.Values.Get(spreadsheetId, range);
            var headerResponse = await getHeaderRequest.ExecuteAsync(cancellationToken);
            bool headersExist = headerResponse.Values != null && headerResponse.Values.Count > 0;

            // Prepare rows for appending
            var valueRange = new ValueRange();
            if (!headersExist)
            {
                var headersRow = command.Form.Formulaire.Body.Select(b => b.Titre).Cast<object>().ToList();
                valueRange.Values = new List<IList<object>> { headersRow };
                range = "Feuille 1"; // Adjust range for appending headers
            }

            var responseRow = command.Form.Formulaire.Body.Select(b => b.Respense).Cast<object>().ToList();
            if (valueRange.Values == null)
                valueRange.Values = new List<IList<object>> { responseRow };
            else
                valueRange.Values.Add(responseRow);

            // Append data to the spreadsheet
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
