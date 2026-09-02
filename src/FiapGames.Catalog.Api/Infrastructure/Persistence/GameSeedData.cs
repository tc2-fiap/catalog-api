using FiapGames.Catalog.Api.Domain;

namespace FiapGames.Catalog.Api.Infrastructure.Persistence;

// Every game seeded into a fresh catalog: the original 9 plus 20 added
// directly against the live cluster during development (now folded back
// into source), plus one intentional always-fails demo game (see notes.md
// 63). Split out of Program.cs purely for size.
public static class GameSeedData
{
    public static readonly Game[] Games =
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
            "https://upload.wikimedia.org/wikipedia/en/thumb/4/46/Grand_Theft_Auto_VI.png/500px-Grand_Theft_Auto_VI.png"),

        new("Hades", "Roguelike", "PC", 89.90m, new DateOnly(2020, 9, 17), null,
            "https://cdn.cloudflare.steamstatic.com/steam/apps/1145360/header.jpg"),
        new("Disco Elysium", "RPG", "PC", 99.90m, new DateOnly(2019, 10, 15), null,
            "https://cdn.cloudflare.steamstatic.com/steam/apps/632470/header.jpg"),
        new("Outer Wilds", "Adventure", "PC", 79.90m, new DateOnly(2019, 5, 28), null,
            "https://cdn.cloudflare.steamstatic.com/steam/apps/753640/header.jpg"),
        new("Dead Cells", "Roguelike", "PC", 59.90m, new DateOnly(2018, 8, 7), null,
            "https://cdn.cloudflare.steamstatic.com/steam/apps/588650/header.jpg"),
        new("Slay the Spire", "Roguelike", "PC", 49.90m, new DateOnly(2019, 1, 23), null,
            "https://cdn.cloudflare.steamstatic.com/steam/apps/646570/header.jpg"),
        new("Return of the Obra Dinn", "Puzzle", "PC", 69.90m, new DateOnly(2018, 10, 18), null,
            "https://cdn.cloudflare.steamstatic.com/steam/apps/653530/header.jpg"),
        new("Inscryption", "Roguelike", "PC", 64.90m, new DateOnly(2021, 10, 19), null,
            "https://cdn.cloudflare.steamstatic.com/steam/apps/1092790/header.jpg"),
        new("Untitled Goose Game", "Puzzle", "PC", 39.90m, new DateOnly(2019, 9, 20), null,
            "https://cdn.cloudflare.steamstatic.com/steam/apps/837470/header.jpg"),
        new("Ori and the Blind Forest", "Platformer", "PC", 74.90m, new DateOnly(2015, 3, 11), null,
            "https://cdn.cloudflare.steamstatic.com/steam/apps/261570/header.jpg"),
        new("Cuphead", "Platformer", "PC", 69.90m, new DateOnly(2017, 9, 29), null,
            "https://cdn.cloudflare.steamstatic.com/steam/apps/268910/header.jpg"),
        new("Divinity Original Sin 2", "RPG", "PC", 129.90m, new DateOnly(2017, 9, 14), null,
            "https://cdn.cloudflare.steamstatic.com/steam/apps/435150/header.jpg"),
        new("Baldur's Gate 3", "RPG", "PC", 199.90m, new DateOnly(2023, 8, 3), null,
            "https://cdn.cloudflare.steamstatic.com/steam/apps/1086940/header.jpg"),
        new("Hi-Fi Rush", "Rhythm", "PC", 129.90m, new DateOnly(2023, 1, 25), null,
            "https://cdn.cloudflare.steamstatic.com/steam/apps/1817230/header.jpg"),
        new("It Takes Two", "Adventure", "PC", 99.90m, new DateOnly(2021, 3, 26), null,
            "https://cdn.cloudflare.steamstatic.com/steam/apps/1426210/header.jpg"),
        new("A Plague Tale Requiem", "Adventure", "PC", 179.90m, new DateOnly(2022, 10, 18), null,
            "https://cdn.cloudflare.steamstatic.com/steam/apps/1182900/header.jpg"),
        new("Death Stranding", "Action", "PC", 149.90m, new DateOnly(2019, 11, 8), null,
            "https://cdn.cloudflare.steamstatic.com/steam/apps/1850570/header.jpg"),
        new("Control", "Action", "PC", 89.90m, new DateOnly(2019, 8, 27), null,
            "https://cdn.cloudflare.steamstatic.com/steam/apps/870780/header.jpg"),
        new("Sekiro Shadows Die Twice", "Action", "PC", 159.90m, new DateOnly(2019, 3, 22), null,
            "https://cdn.cloudflare.steamstatic.com/steam/apps/814380/header.jpg"),
        new("Dark Souls III", "RPG", "PC", 99.90m, new DateOnly(2016, 4, 12), null,
            "https://cdn.cloudflare.steamstatic.com/steam/apps/374320/header.jpg"),
        new("Monster Hunter World", "Action", "PC", 119.90m, new DateOnly(2018, 8, 9), null,
            "https://cdn.cloudflare.steamstatic.com/steam/apps/582010/header.jpg"),

        // Deterministic always-fails demo game — SimulatedPaymentGateway
        // rejects any price ending in .13 cents (bdd.md line 305 already
        // documents 49.13 as a Rejected example). Priced here for exactly
        // that reason, so a demo/grading walkthrough can reliably show a
        // Failed order without relying on randomness. No cover image: it
        // isn't a real game, so it intentionally falls back to the
        // letter-tile placeholder.
        new("Corrupted Save: QA Edition", "Simulation", "PC", 49.13m, new DateOnly(2024, 1, 1),
            "A deliberately-priced demo game: this purchase always fails at payment, for exercising the Failed order path on demand.",
            null),
    ];
}
