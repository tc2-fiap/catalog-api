using FiapGames.Catalog.Api.Application.Abstractions;
using FiapGames.Catalog.Api.Domain;
using FiapGames.Shared.Kernel.Pagination;
using Microsoft.EntityFrameworkCore;

namespace FiapGames.Catalog.Api.Infrastructure.Persistence;

public sealed class GameRepository : IGameRepository
{
    private readonly GamesDbContext _context;

    public GameRepository(GamesDbContext context)
    {
        _context = context;
    }

    public Task<Game?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _context.Games.FirstOrDefaultAsync(g => g.Id == id, cancellationToken);

    public async Task<PagedResult<Game>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default)
    {
        var query = _context.Games.OrderBy(g => g.CreatedAtUtc);

        var totalCount = await query.LongCountAsync(cancellationToken);
        var items = await query.Skip(request.Skip).Take(request.PageSize ?? 10).ToListAsync(cancellationToken);

        return new PagedResult<Game>(items, totalCount, request.Page ?? 1, request.PageSize ?? 10);
    }

    public async Task<PagedResult<Game>> SearchPagedAsync(
        PagedRequest request,
        string? title,
        string? genre,
        string? platform,
        decimal? minPrice,
        decimal? maxPrice,
        string? sortBy,
        string? sortDir,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Games.AsQueryable();

        if (!string.IsNullOrWhiteSpace(title))
            query = query.Where(g => EF.Functions.ILike(g.Title, $"%{title}%"));
        if (!string.IsNullOrWhiteSpace(genre))
            query = query.Where(g => g.Genre == genre);
        if (!string.IsNullOrWhiteSpace(platform))
            query = query.Where(g => g.Platform == platform);
        if (minPrice.HasValue)
            query = query.Where(g => g.Price >= minPrice.Value);
        if (maxPrice.HasValue)
            query = query.Where(g => g.Price <= maxPrice.Value);

        var descending = string.Equals(sortDir, "desc", StringComparison.OrdinalIgnoreCase);
        query = sortBy?.ToLowerInvariant() switch
        {
            "price" => descending ? query.OrderByDescending(g => g.Price) : query.OrderBy(g => g.Price),
            "platform" => descending ? query.OrderByDescending(g => g.Platform) : query.OrderBy(g => g.Platform),
            "genre" => descending ? query.OrderByDescending(g => g.Genre) : query.OrderBy(g => g.Genre),
            "title" => descending ? query.OrderByDescending(g => g.Title) : query.OrderBy(g => g.Title),
            _ => descending ? query.OrderByDescending(g => g.CreatedAtUtc) : query.OrderBy(g => g.CreatedAtUtc),
        };

        var totalCount = await query.LongCountAsync(cancellationToken);
        var items = await query.Skip(request.Skip).Take(request.PageSize ?? 10).ToListAsync(cancellationToken);

        return new PagedResult<Game>(items, totalCount, request.Page ?? 1, request.PageSize ?? 10);
    }

    public Task AddAsync(Game entity, CancellationToken cancellationToken = default)
    {
        _context.Games.Add(entity);
        return Task.CompletedTask;
    }

    public void Update(Game entity) => _context.Games.Update(entity);

    public void Remove(Game entity) => _context.Games.Remove(entity);

    public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await _context.SaveChangesAsync(cancellationToken) >= 0;
}
