using FiapGames.Catalog.Api.Application.Dtos;
using FiapGames.Shared.Kernel.Results;

namespace FiapGames.Catalog.Api.Application.Abstractions;

public interface IQuotationService
{
    Task<Result<QuotationResponse>> GetUsdToBrlAsync(CancellationToken cancellationToken = default);
}
