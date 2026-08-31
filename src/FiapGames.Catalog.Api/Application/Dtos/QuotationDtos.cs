namespace FiapGames.Catalog.Api.Application.Dtos;

public sealed record QuotationResponse(decimal UsdToBrlRate, DateOnly AsOf, string Source);
