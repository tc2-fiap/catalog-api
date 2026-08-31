using FiapGames.Catalog.Api.Application.Abstractions;
using FiapGames.Catalog.Api.Application.Services;
using FiapGames.Shared.Kernel.Results;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace FiapGames.Catalog.Tests;

public class QuotationServiceTests
{
    private readonly IQuotationProvider _frankfurter = Substitute.For<IQuotationProvider>();
    private readonly IQuotationProvider _exchangeRateApi = Substitute.For<IQuotationProvider>();
    private readonly IMemoryCache _cache = new MemoryCache(new MemoryCacheOptions());
    private readonly QuotationService _sut;

    public QuotationServiceTests()
    {
        _frankfurter.Name.Returns("frankfurter");
        _exchangeRateApi.Name.Returns("exchangerate-api");

        var configuration = new ConfigurationBuilder().Build();
        var logger = Substitute.For<ILogger<QuotationService>>();
        _sut = new QuotationService([_frankfurter, _exchangeRateApi], _cache, configuration, logger);
    }

    [Fact]
    public async Task GetUsdToBrlAsync_WhenFrankfurterSucceeds_UsesItAndSkipsFallback()
    {
        _frankfurter.GetUsdToBrlRateAsync(Arg.Any<CancellationToken>()).Returns(5.42m);

        var result = await _sut.GetUsdToBrlAsync();

        Assert.True(result.IsSuccess);
        Assert.Equal(5.42m, result.Value.UsdToBrlRate);
        Assert.Equal("frankfurter", result.Value.Source);
        await _exchangeRateApi.DidNotReceive().GetUsdToBrlRateAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetUsdToBrlAsync_WhenFrankfurterFails_FallsBackToExchangeRateApi()
    {
        _frankfurter.GetUsdToBrlRateAsync(Arg.Any<CancellationToken>()).Returns((decimal?)null);
        _exchangeRateApi.GetUsdToBrlRateAsync(Arg.Any<CancellationToken>()).Returns(5.50m);

        var result = await _sut.GetUsdToBrlAsync();

        Assert.True(result.IsSuccess);
        Assert.Equal(5.50m, result.Value.UsdToBrlRate);
        Assert.Equal("exchangerate-api", result.Value.Source);
    }

    [Fact]
    public async Task GetUsdToBrlAsync_WhenBothProvidersFail_ReturnsConflict()
    {
        _frankfurter.GetUsdToBrlRateAsync(Arg.Any<CancellationToken>()).Returns((decimal?)null);
        _exchangeRateApi.GetUsdToBrlRateAsync(Arg.Any<CancellationToken>()).Returns((decimal?)null);

        var result = await _sut.GetUsdToBrlAsync();

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.Conflict, result.Error!.Type);
    }

    [Fact]
    public async Task GetUsdToBrlAsync_SecondCallWithinTtl_IsCacheServedAndSkipsProviders()
    {
        _frankfurter.GetUsdToBrlRateAsync(Arg.Any<CancellationToken>()).Returns(5.42m);

        await _sut.GetUsdToBrlAsync();
        var second = await _sut.GetUsdToBrlAsync();

        Assert.True(second.IsSuccess);
        Assert.Equal(5.42m, second.Value.UsdToBrlRate);
        await _frankfurter.Received(1).GetUsdToBrlRateAsync(Arg.Any<CancellationToken>());
    }
}
