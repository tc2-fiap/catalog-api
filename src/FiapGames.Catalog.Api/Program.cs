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
        Game[] seedGames =
        [
            new("The Witcher 3: Wild Hunt", "RPG", "PC", 79.99m, new DateOnly(2015, 5, 18),
                "An open-world RPG following monster hunter Geralt of Rivia.",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/292030/header.jpg"),
            new("Hollow Knight", "Metroidvania", "PC", 34.99m, new DateOnly(2017, 2, 24),
                "A challenging 2D action-adventure through a vast ruined kingdom of insects.",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/367520/header.jpg"),
            new("Stardew Valley", "Simulation", "PC", 24.99m, new DateOnly(2016, 2, 26),
                "Inherit your grandfather's old farm and start a new life in the countryside.",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/413150/header.jpg"),
            new("Portal 2", "Puzzle", "PC", 39.99m, new DateOnly(2011, 4, 19),
                "A first-person puzzle-platformer built around a physics-bending portal gun.",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/620/header.jpg"),
            new("Celeste", "Platformer", "PC", 29.99m, new DateOnly(2018, 1, 25),
                "A tightly-designed precision platformer about climbing a mountain.",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/504230/header.jpg"),
            new("Terraria", "Sandbox", "PC", 19.99m, new DateOnly(2011, 5, 16),
                "A 2D sandbox adventure of building, exploration, and combat.",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/105600/header.jpg"),
            new("Elden Ring", "Action RPG", "PS5", 249.90m, new DateOnly(2022, 2, 25),
                "An open-world action RPG set in the Lands Between.",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/1245620/header.jpg"),
            new("Cyberpunk 2077", "Action RPG", "Xbox", 199.90m, new DateOnly(2020, 12, 10),
                "An open-world action RPG set in the dystopian metropolis of Night City.",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/1091500/header.jpg"),
            new("Grand Theft Auto VI", "Action-Adventure", "PS5", 349.90m, new DateOnly(2026, 11, 19),
                "An open-world action-adventure set across Leonida, including a fictionalized Miami.",
                "https://upload.wikimedia.org/wikipedia/en/thumb/4/46/Grand_Theft_Auto_VI.png/500px-Grand_Theft_Auto_VI.png")
        ];

        foreach (var game in seedGames)
            await repository.AddAsync(game);

        await repository.SaveChangesAsync();

        Log.Information("Seeded catalog with {Count} games", seedGames.Length);
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
