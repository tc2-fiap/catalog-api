[English](README.en-US.md) · **Português**

# FIAP Games — Catalog API

O catálogo de jogos — apenas dado de referência de produto. Dono do schema `catalog` no Postgres. Não publica nem consome eventos; fica completamente fora do fluxo de eventos de compra (`../documentation/spec/notes.md` 1).

## Rodar de forma independente

```bash
cp .env.example .env
docker compose up --build
```

Sobe este serviço mais seu próprio Postgres. Sem RabbitMQ — este é o único repositório de backend que não troca mensagens. API em `localhost:8083`, Swagger em `/swagger`.

## Rodar como parte do sistema

Implantado pelo chart Helm [`orchestration`](https://github.com/tc2-fiap/orchestration) junto com os outros quatro serviços de backend e o frontend — ver [`../orchestration/README.pt-BR.md`](../orchestration/README.pt-BR.md). Acessado pelo Ingress compartilhado em `/api/games/*` e `/api/quotations/*`.

## O que tem aqui

- `Domain/Game.cs` — Id, Title, Genre, Platform, Description, Price, ReleaseDate, CoverImageUrl (anulável, só para exibição — ver `../documentation/spec/notes.md` 41).
- CRUD completo com paginação e FluentValidation, protegido por JWT como todo outro serviço (um token emitido pelo [`users-api`](https://github.com/tc2-fiap/users-api) é aceito aqui sem nenhuma configuração compartilhada além do segredo de assinatura idêntico).
- Se auto-semeia com 8 jogos reais (capas reais, preços realistas em BRL) na inicialização, se o catálogo estiver vazio — idempotente, nunca re-semeia nem desfaz edições de admin.
- O [`orders-api`](https://github.com/tc2-fiap/orders-api) lê o preço de um jogo de forma síncrona daqui (`GET /api/games/{id}`) quando uma compra é feita — a única chamada síncrona em um sistema por outro lado orientado a eventos (`instructions.md` §6).
- `GET /api/quotations/usd-brl` — uma cotação USD→BRL ao vivo (Frankfurter, com fallback para ExchangeRate-API), cacheada em memória por uma hora. Usada só para exibição: o frontend converte o preço em BRL de um jogo para USD quando o idioma da interface está em inglês, e nenhum campo de preço no backend muda de significado (`../documentation/spec/notes.md` 39).

## Testar

```bash
cd tests/FiapGames.Catalog.Tests && dotnet test
```

## Documentação

A arquitetura completa, os contratos de eventos e o registro de decisões do projeto vivem em [`../documentation/`](../documentation/) — ver [`DOCUMENTATION.pt-BR.md`](../documentation/narrative/DOCUMENTATION.pt-BR.md) e [`instructions.md`](../documentation/spec/instructions.md) §4.2 (em inglês).
