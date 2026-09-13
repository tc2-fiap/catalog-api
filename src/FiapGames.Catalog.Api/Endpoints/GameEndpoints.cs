using FiapGames.Catalog.Api.Application.Abstractions;
using FiapGames.Catalog.Api.Application.Dtos;
using FiapGames.Shared.Infrastructure.Extensions;
using FiapGames.Shared.Kernel.Pagination;
using FluentValidation;

namespace FiapGames.Catalog.Api.Endpoints;

public static class GameEndpoints
{
    public static IEndpointRouteBuilder MapGameEndpoints(this IEndpointRouteBuilder endpoints, Func<IResult> getVersion)
    {
        var group = endpoints.MapGroup("/api/catalog").WithTags("Games").RequireAuthorization();

        group.MapPost("/", async (
            CreateGameRequest request,
            IValidator<CreateGameRequest> validator,
            IGameService service,
            CancellationToken cancellationToken) =>
        {
            var validation = await validator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
                return Results.ValidationProblem(validation.ToDictionary());

            var game = await service.CreateAsync(request, cancellationToken);
            return Results.Created($"/api/catalog/{game.Id}", game);
        }).RequireAuthorization(p => p.RequireRole("Admin"));

        group.MapGet("/{id:guid}", async (Guid id, IGameService service, CancellationToken cancellationToken) =>
        {
            var result = await service.GetByIdAsync(id, cancellationToken);
            return result.ToHttpResult();
        });

        group.MapGet("/", async (
            [AsParameters] PagedRequest request,
            string? title,
            string? genre,
            string? platform,
            decimal? minPrice,
            decimal? maxPrice,
            string? sortBy,
            string? sortDir,
            IGameService service,
            CancellationToken cancellationToken) =>
        {
            var result = await service.SearchAsync(request, title, genre, platform, minPrice, maxPrice, sortBy, sortDir, cancellationToken);
            return Results.Ok(result);
        });

        group.MapPut("/{id:guid}", async (
            Guid id,
            UpdateGameRequest request,
            IValidator<UpdateGameRequest> validator,
            IGameService service,
            CancellationToken cancellationToken) =>
        {
            var validation = await validator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
                return Results.ValidationProblem(validation.ToDictionary());

            var result = await service.UpdateAsync(id, request, cancellationToken);
            return result.ToHttpResult();
        }).RequireAuthorization(p => p.RequireRole("Admin"));

        group.MapDelete("/{id:guid}", async (Guid id, IGameService service, CancellationToken cancellationToken) =>
        {
            var result = await service.DeleteAsync(id, cancellationToken);
            return result.ToHttpResult();
        }).RequireAuthorization(p => p.RequireRole("Admin"));

        // Admin-dashboard-facing twin of the bare /version (see Program.cs):
        // same handler, reached via the Ingress like any other route in
        // this group instead of only via kubectl port-forward, gated to Admin.
        group.MapGet("/version", getVersion).RequireAuthorization(p => p.RequireRole("Admin"));

        return endpoints;
    }
}
