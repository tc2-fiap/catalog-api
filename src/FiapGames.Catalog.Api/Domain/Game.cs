using FiapGames.Shared.Kernel.Entities;

namespace FiapGames.Catalog.Api.Domain;

public class Game : Entity
{
    public string Title { get; private set; } = string.Empty;

    public string Genre { get; private set; } = string.Empty;

    public string Platform { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public string? CoverImageUrl { get; private set; }

    public decimal Price { get; private set; }

    public DateOnly ReleaseDate { get; private set; }

    private Game() { }

    public Game(string title, string genre, string platform, decimal price, DateOnly releaseDate, string? description = null, string? coverImageUrl = null)
    {
        Title = title;
        Genre = genre;
        Platform = platform;
        Price = price;
        ReleaseDate = releaseDate;
        Description = description;
        CoverImageUrl = coverImageUrl;
    }

    public void UpdateDetails(string title, string genre, string platform, decimal price, DateOnly releaseDate, string? description, string? coverImageUrl)
    {
        Title = title;
        Genre = genre;
        Platform = platform;
        Price = price;
        ReleaseDate = releaseDate;
        Description = description;
        CoverImageUrl = coverImageUrl;
        Touch();
    }
}
