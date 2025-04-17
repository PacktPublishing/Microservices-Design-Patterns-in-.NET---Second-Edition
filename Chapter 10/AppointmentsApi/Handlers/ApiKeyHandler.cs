using System;

namespace AppointmentsApi.Handlers;


public class ApiKeyHandler(IConfiguration _configuration) : DelegatingHandler
{
    private string _headerName = "x-api-key";

    private string _apiKey = _configuration["ApiKey"];

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (!request.Headers.Contains(_headerName))
        {
            request.Headers.Add(_headerName, _apiKey);
        }

        return base.SendAsync(request, cancellationToken);
    }
}