namespace FiapGames.Catalog.Api.Application.Abstractions;

// A single upstream USD->BRL rate source. Never throws for "unreachable" —
// returns null so QuotationService can fall through to the next provider.
public interface IQuotationProvider
{
    string Name { get; }

    Task<decimal?> GetUsdToBrlRateAsync(CancellationToken cancellationToken = default);
}
