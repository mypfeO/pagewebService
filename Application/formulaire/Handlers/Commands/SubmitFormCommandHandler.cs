using MediatR;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domaine.Entities;
using Application.formulaire.Commands;
using FluentResults; // Assurez-vous d'inclure votre espace de noms pour Domaine.Entities
using System.Text.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

public class SubmitFormCommandHandler : IRequestHandler<SubmitFormCommand, Result<string>>
{
    private readonly GraphServiceClient _graphClient;

    public SubmitFormCommandHandler(GraphServiceClient graphClient)
    {
        _graphClient = graphClient;
    }

    public Task<Result<string>> Handle(SubmitFormCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    // Assurez-vous d'avoir 'using Microsoft.Graph;'

    public async Task<Result<string>> UpdateExcelRangeAsync(string accessToken, string fileId, string range, List<object[]> values)
{
    var httpClient = new HttpClient();
    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

    var requestUri = $"https://graph.microsoft.com/v1.0/me/drive/items/{fileId}/workbook/worksheets/Feuille1/range(address='{range}')";

    var requestBody = new
    {
        values = values
    };

    var httpContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

    var response = await httpClient.PatchAsync(requestUri, httpContent);

    if (response.IsSuccessStatusCode)
    {
        return Result.Ok("La plage Excel a été mise à jour avec succès.");
    }
    else
    {
        return Result.Fail($"Erreur lors de la mise à jour de la plage Excel : {response.StatusCode}");
    }
}




private string ExtractFileIdFromLink(string excelFileLink)
    {
        // Implémentez cette fonction pour extraire correctement l'ID du fichier depuis l'URL fournie dans ExcelFileLink
        // Cette extraction dépend de la structure de votre URL
        throw new NotImplementedException();
    }
}
