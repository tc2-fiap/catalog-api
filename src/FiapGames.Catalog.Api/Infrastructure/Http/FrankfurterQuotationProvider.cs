using System.Net.Http.Json;
using System.Text.Json;
using FiapGames.Catalog.Api.Application.Abstractions;

namespace FiapGames.Catalog.Api.Infrastructure.Http;

// Free, keyless USD->BRL rate lookup — the primary provider. Live-verified
// route: docs/features/quotation-feature.md's v2/rate/USD/BRL?amount=
// example returns 422 ("unknown parameter: amount") against the real API
// today — that doc's example was stale. The actual working shape is
// v1/latest?base=USD&symbols=BRL, structurally the same rates-object shape
// ExchangeRateApiQuotationProvider already parses. See QuotationService
// for fallback ordering.
public sealed class FrankfurterQuotationProvider : IQuotationProvider
{
    public string Name => "frankfurter";

    private readonly HttpClient _httpClient;
    private readonly ILogger<FrankfurterQuotationProvider> _logger;

    public FrankfurterQuotationProvider(HttpClient httpClient, ILogger<FrankfurterQuotationProvider> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<decimal?> GetUsdToBrlRateAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<FrankfurterLatestResponse>("v1/latest?base=USD&symbols=BRL", cancellationToken);
            return response?.Rates is not null && response.Rates.TryGetValue("BRL", out var rate) ? rate : null;
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException)
        {
            _logger.LogWarning(ex, "Frankfurter quotation lookup failed");
            return null;
        }
    }

    private sealed record FrankfurterLatestResponse(Dictionary<string, decimal>? Rates);
}
