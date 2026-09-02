using FiapGames.Catalog.Api.Domain;
using FiapGames.Shared.Kernel.Pagination;
using FiapGames.Shared.Kernel.Repositories;

namespace FiapGames.Catalog.Api.Application.Abstractions;

public interface IGameRepository : IRepository<Game>
{
    Task<PagedResult<Game>> SearchPagedAsync(
        PagedRequest request,
        string? title,
        string? genre,
        string? platform,
        decimal? minPrice,
        decimal? maxPrice,
        string? sortBy,
        string? sortDir,
        CancellationToken cancellationToken = default);
}
