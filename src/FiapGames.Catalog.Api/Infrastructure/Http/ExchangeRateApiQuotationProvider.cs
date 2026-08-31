using System.Net.Http.Json;
using System.Text.Json;
using FiapGames.Catalog.Api.Application.Abstractions;

namespace FiapGames.Catalog.Api.Infrastructure.Http;

// Fallback keyless USD->BRL rate lookup, tried only when
// FrankfurterQuotationProvider is unavailable. See QuotationService.
public sealed class ExchangeRateApiQuotationProvider : IQuotationProvider
{
    public string Name => "exchangerate-api";

    private readonly HttpClient _httpClient;
    private readonly ILogger<ExchangeRateApiQuotationProvider> _logger;

    public ExchangeRateApiQuotationProvider(HttpClient httpClient, ILogger<ExchangeRateApiQuotationProvider> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<decimal?> GetUsdToBrlRateAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<ExchangeRateApiResponse>("v6/latest/USD", cancellationToken);
            return response?.Rates is not null && response.Rates.TryGetValue("BRL", out var rate) ? rate : null;
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException)
        {
            _logger.LogWarning(ex, "ExchangeRate-API quotation lookup failed");
            return null;
        }
    }

    private sealed record ExchangeRateApiResponse(Dictionary<string, decimal>? Rates);
}
