using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.GoogleService
{
    public interface IGoogleSheetsService
    {
        Task AppendEntryAsync(string spreadsheetId, IList<IList<object>> values);
    }
}
