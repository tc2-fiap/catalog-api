using FiapGames.Catalog.Api.Application.Abstractions;
using FiapGames.Shared.Infrastructure.Extensions;

namespace FiapGames.Catalog.Api.Endpoints;

public static class QuotationEndpoints
{
    public static IEndpointRouteBuilder MapQuotationEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/quotations").WithTags("Quotations").RequireAuthorization();

        group.MapGet("/usd-brl", async (IQuotationService service, CancellationToken cancellationToken) =>
        {
            var result = await service.GetUsdToBrlAsync(cancellationToken);
            return result.ToHttpResult();
        });

        return endpoints;
    }
}
