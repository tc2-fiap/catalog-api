[English](README.en-US.md) · **Português**

# FIAP Games — Catalog API

O catálogo de jogos — apenas dado de referência de produto. Dono do schema `catalog` no Postgres. Não publica eventos do fluxo de compra e não consome nenhum; fica completamente fora desse fluxo (`../documentation/spec/notes.md` 1). Ele consome o `TokenRevokedEvent` — um evento transversal de infraestrutura de autenticação, sem relação com compras (`../documentation/spec/notes.md` 84).

## Rodar de forma independente

```bash
cp .env.example .env
docker compose up --build
```

Sobe este serviço mais seu próprio Postgres. API em `localhost:8083`, Swagger em `/swagger`.

## Rodar como parte do sistema

Implantado pelo chart Helm [`orchestration`](https://github.com/tc2-fiap/orchestration) junto com os outros serviços de backend e o frontend — ver [`../orchestration/README.pt-BR.md`](../orchestration/README.pt-BR.md). Acessado pelo Ingress compartilhado em `/api/catalog/*` e `/api/quotations/*`.

## O que tem aqui

- `Domain/Game.cs` — Id, Title, Genre, Platform, Description, Price, ReleaseDate, CoverImageUrl (anulável, só para exibição — ver `../documentation/spec/notes.md` 41).
- CRUD completo com paginação e FluentValidation, protegido por JWT como todo outro serviço (um token emitido pelo [`users-api`](https://github.com/tc2-fiap/users-api) é aceito aqui sem nenhuma configuração compartilhada além do segredo de assinatura idêntico).
- Se auto-semeia com 8 jogos reais (capas reais, preços realistas em BRL) na inicialização, se o catálogo estiver vazio — idempotente, nunca re-semeia nem desfaz edições de admin.
- O [`orders-api`](https://github.com/tc2-fiap/orders-api) lê o preço de um jogo de forma síncrona daqui (`GET /api/catalog/{id}`) quando uma compra é feita — a única chamada síncrona em um sistema por outro lado orientado a eventos (`instructions.md` §6).
- `GET /api/quotations/usd-brl` — uma cotação USD→BRL ao vivo (Frankfurter, com fallback para ExchangeRate-API), cacheada em memória por uma hora. Usada só para exibição: o frontend converte o preço em BRL de um jogo para USD quando o idioma da interface está em inglês, e nenhum campo de preço no backend muda de significado (`../documentation/spec/notes.md` 39).

## Testar

```bash
cd tests/FiapGames.Catalog.Tests && dotnet test
```

## Documentação

A arquitetura completa, os contratos de eventos e o registro de decisões do projeto vivem no repositório `documentation` — [`github.com/tc2-fiap/documentation`](https://github.com/tc2-fiap/documentation) (ou `../documentation/` se você o tiver clonado como irmão) — ver [`ARCHITECTURE.pt-BR.md`](https://github.com/tc2-fiap/documentation/blob/main/architecture/ARCHITECTURE.pt-BR.md) e [`instructions.md`](https://github.com/tc2-fiap/documentation/blob/main/spec/instructions.md) §4.2 (em inglês).
