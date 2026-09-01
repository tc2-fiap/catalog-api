**English** · [Português](README.pt-BR.md)

# FIAP Games — Catalog API

The game catalog — product reference data only. Owns the `catalog` Postgres schema. Publishes no events and consumes none; it sits entirely outside the purchase event flow (`../documentation/spec/notes.md` 1).

## Run standalone

```bash
cp .env.example .env
docker compose up --build
```

Brings up this service plus its own Postgres. No RabbitMQ — this is the one backend repo that doesn't message. API on `localhost:8083`, Swagger at `/swagger`.

## Run as part of the system

Deployed by the [`orchestration`](https://github.com/tc2-fiap/orchestration) Helm chart alongside the other four backend services and the frontend — see [`../orchestration/README.en-US.md`](../orchestration/README.en-US.md). Reached through the shared Ingress at `/api/games/*` and `/api/quotations/*`.

## What's here

- `Domain/Game.cs` — Id, Title, Genre, Platform, Description, Price, ReleaseDate, CoverImageUrl (nullable, display-only — see `../documentation/spec/notes.md` 41).
- Full CRUD with pagination and FluentValidation, JWT-protected like every other service (a token minted by [`users-api`](https://github.com/tc2-fiap/users-api) is accepted here without any shared config beyond the identical signing secret).
- Seeds itself with 8 real games (real cover art, realistic BRL prices) at startup if the catalog is empty — idempotent, never re-seeds or resets admin edits.
- [`orders-api`](https://github.com/tc2-fiap/orders-api) reads a game's price synchronously from here (`GET /api/games/{id}`) when a purchase is placed — the one synchronous call in an otherwise event-driven system (`instructions.md` §6).
- `GET /api/quotations/usd-brl` — a live USD→BRL rate (Frankfurter, falling back to ExchangeRate-API), cached in-memory for an hour. Used only for display: the frontend converts a game's BRL price to USD when the language toggle is English, and no backend price field ever changes meaning (`../documentation/spec/notes.md` 39).

## Test

```bash
cd tests/FiapGames.Catalog.Tests && dotnet test
```

## Documentation

Full architecture, event contracts, and the project-wide decision record live in the `documentation` repo — [`github.com/tc2-fiap/documentation`](https://github.com/tc2-fiap/documentation) (or `../documentation/` if you have it cloned as a sibling) — see [`ARCHITECTURE.en-US.md`](https://github.com/tc2-fiap/documentation/blob/main/architecture/ARCHITECTURE.en-US.md) and [`instructions.md`](https://github.com/tc2-fiap/documentation/blob/main/spec/instructions.md) §4.2.
