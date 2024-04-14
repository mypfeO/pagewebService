using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Sheets.v4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.GoogleService
{
    public class GoogleSheetsService : IGoogleSheetsService
    {
        private readonly SheetsService _sheetsService;

        public GoogleSheetsService(string apiKey)
        {
            var credential = GoogleCredential.FromFile(apiKey)
                .CreateScoped(SheetsService.Scope.Spreadsheets);
            _sheetsService = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Google Sheets API Application",
            });
        }

        public async Task AppendEntryAsync(string spreadsheetId, IList<IList<object>> values)
        {
            var range = "Sheet1"; // Define the appropriate range based on your needs
            var valueRange = new ValueRange { Values = values };
            var request = _sheetsService.Spreadsheets.Values.Append(valueRange, spreadsheetId, range);
            request.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
            await request.ExecuteAsync();
        }
    }
}
