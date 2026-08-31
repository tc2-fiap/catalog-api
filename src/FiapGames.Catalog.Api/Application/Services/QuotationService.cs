using FiapGames.Catalog.Api.Application.Abstractions;
using FiapGames.Catalog.Api.Application.Dtos;
using FiapGames.Shared.Kernel.Results;
using Microsoft.Extensions.Caching.Memory;

namespace FiapGames.Catalog.Api.Application.Services;

public sealed class QuotationService : IQuotationService
{
    private const string CacheKey = "quotation:usd-brl";

    // Tried in registration order — Frankfurter first, ExchangeRate-API as
    // fallback. See Program.cs for how that order is wired.
    private readonly IReadOnlyList<IQuotationProvider> _providers;
    private readonly IMemoryCache _cache;
    private readonly TimeSpan _cacheTtl;
    private readonly ILogger<QuotationService> _logger;

    public QuotationService(
        IEnumerable<IQuotationProvider> providers,
        IMemoryCache cache,
        IConfiguration configuration,
        ILogger<QuotationService> logger)
    {
        _providers = providers.ToList();
        _cache = cache;
        _cacheTtl = TimeSpan.FromMinutes(configuration.GetValue("Quotation:CacheTtlMinutes", 60));
        _logger = logger;
    }

    public async Task<Result<QuotationResponse>> GetUsdToBrlAsync(CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue(CacheKey, out QuotationResponse? cached) && cached is not null)
            return Result.Success(cached);

        foreach (var provider in _providers)
        {
            var rate = await provider.GetUsdToBrlRateAsync(cancellationToken);
            if (rate is null)
                continue;

            var response = new QuotationResponse(rate.Value, DateOnly.FromDateTime(DateTime.UtcNow), provider.Name);
            _cache.Set(CacheKey, response, _cacheTtl);
            return Result.Success(response);
        }

        _logger.LogWarning("All quotation providers failed to resolve a USD-BRL rate");
        return Result.Failure<QuotationResponse>(Error.Conflict("Exchange rate is currently unavailable."));
    }
}
