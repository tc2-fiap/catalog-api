using FiapGames.Catalog.Api.Application.Abstractions;
using FiapGames.Catalog.Api.Application.Services;
using FiapGames.Catalog.Api.Application.Validators;
using FiapGames.Catalog.Api.Domain;
using FiapGames.Catalog.Api.Endpoints;
using FiapGames.Catalog.Api.Infrastructure.Http;
using FiapGames.Catalog.Api.Infrastructure.Persistence;
using FiapGames.Shared.Infrastructure.Extensions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Serilog;
using Serilog.Formatting.Compact;

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console(new CompactJsonFormatter())
    .CreateBootstrapLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console(new CompactJsonFormatter()));

// Built from parts, not a single ConnectionStrings entry, so only the
// password needs to come from a Kubernetes Secret — host/port/database/
// username/schema are non-secret ConfigMap values.
var postgresConnectionString =
    $"Host={builder.Configuration["Postgres:Host"] ?? "localhost"};" +
    $"Port={builder.Configuration["Postgres:Port"] ?? "5432"};" +
    $"Database={builder.Configuration["Postgres:Database"] ?? "fiap_games"};" +
    $"Username={builder.Configuration["Postgres:Username"] ?? "catalog_role"};" +
    $"Password={builder.Configuration["Postgres:Password"]};" +
    $"Search Path={builder.Configuration["Postgres:SearchPath"] ?? "catalog"}";

builder.Services.AddDbContext<GamesDbContext>(options =>
    options.UseNpgsql(postgresConnectionString));

builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddGlobalExceptionHandling();

builder.Services.AddScoped<IGameRepository, GameRepository>();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateGameRequestValidator>();

// Frankfurter tried first, ExchangeRate-API as fallback — order comes from
// IQuotationProvider registration order below. See notes.md 39.
builder.Services.AddHttpClient<FrankfurterQuotationProvider>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Quotation:FrankfurterBaseUrl"] ?? "https://api.frankfurter.dev");
    client.Timeout = TimeSpan.FromSeconds(5);
});
builder.Services.AddHttpClient<ExchangeRateApiQuotationProvider>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Quotation:ExchangeRateApiBaseUrl"] ?? "https://open.er-api.com");
    client.Timeout = TimeSpan.FromSeconds(5);
});
builder.Services.AddScoped<IQuotationProvider>(sp => sp.GetRequiredService<FrankfurterQuotationProvider>());
builder.Services.AddScoped<IQuotationProvider>(sp => sp.GetRequiredService<ExchangeRateApiQuotationProvider>());
builder.Services.AddMemoryCache();
builder.Services.AddScoped<IQuotationService, QuotationService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "FIAP Games — Catalog API", Version = "v1" });

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter a valid JWT token."
    };
    options.AddSecurityDefinition("Bearer", securityScheme);
    options.AddSecurityRequirement(document =>
    {
        var requirement = new OpenApiSecurityRequirement();
        requirement.Add(new OpenApiSecuritySchemeReference("Bearer", document, null), []);
        return requirement;
    });
});

builder.Services.AddHealthChecks();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<GamesDbContext>();
    db.Database.Migrate();

    // Bootstrap: an empty catalog makes for an empty demo (GETTING_STARTED's
    // walkthrough browses `GET /api/games` immediately after cluster
    // bring-up). Idempotent — only seeds once, the same "if empty" shape as
    // users-api's admin bootstrap; a restart never re-seeds or resets
    // admin-made edits.
    if (!await db.Games.AnyAsync())
    {
        var repository = scope.ServiceProvider.GetRequiredService<IGameRepository>();

        foreach (var game in GameSeedData.Games)
            await repository.AddAsync(game);

        await repository.SaveChangesAsync();

        Log.Information("Seeded catalog with {Count} games", GameSeedData.Games.Length);
    }
}

app.UseExceptionHandler();

app.UseSerilogRequestLogging();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health");

app.MapGameEndpoints();
app.MapQuotationEndpoints();

try
{
    app.Run();
}
finally
{
    Log.CloseAndFlush();
}

public partial class Program;
