# Plano de construção do backend

De um esqueleto Clean Architecture vazio até a API que sustenta o frontend Angular já pronto.
Stack: **.NET 10 + EF Core + PostgreSQL**.

Este plano foi levantado do código do frontend (`lucasvidalz/licitaedital-web`), não de suposição.
Cada endpoint abaixo tem um `*-api.service.ts` que já o chama e um `*-api.model.ts` que é espelho
literal do contrato. O padrão de implementação está em [`../CLAUDE.md`](../CLAUDE.md).

---

## 1. O que o frontend já espera

### Forma da comunicação — não negociável, já está em produção no cliente

| Item | Valor | Onde |
| --- | --- | --- |
| Base URL em dev | `http://localhost:8080/api` | `frontend/src/environments/environment.development.ts:5` |
| Base URL em homolog/prod | `/api` (mesmo host, atrás de proxy) | `environment.homolog.ts:5`, `environment.production.ts:5` |
| Sessão | **cookie**, `withCredentials: true` em toda chamada | `core/http/interceptors/credentials.interceptor.ts` |
| CSRF | cookie `XSRF-TOKEN` → header `X-XSRF-TOKEN` | `app.config.ts:36-39` |
| Correlação | header `X-Correlation-ID` (UUID por request) | `core/http/interceptors/correlation-id.interceptor.ts:7` |
| Idioma | `Accept-Language: pt-BR` | `core/http/interceptors/request-context.interceptor.ts` |
| 401 | limpa sessão e redireciona para `/login` — em **qualquer** rota, não só `/auth/me` | `core/http/interceptors/error.interceptor.ts:39-45` |
| Paginação | `{ items, page, pageSize, totalItems, totalPages }` | `core/http/models/paged-response.model.ts` |

> ⚠️ **Conflito a decidir antes da Fase 0.** A spec de participação assistida
> (`03-participacao-assistida-especificacao-codex.md` §15) manda prefixo `/api/v1`; o frontend hoje
> chama `/api` sem versão. São incompatíveis. Ver decisão **D-05**.

### Endpoints com contrato firmado — 27, o alvo real

`REST` já chamado pelo Angular. Rota relativa a `apiBaseUrl`.

| # | Método e rota | Resposta | Fase |
| --- | --- | --- | --- |
| 1 | `GET /auth/me` | `AuthUser` ou 401 | 1 |
| 2 | `POST /auth/login` | `AuthUser` + `Set-Cookie` | 1 |
| 3 | `POST /auth/register` | `AuthUser` + `Set-Cookie` | 1 |
| 4 | `POST /auth/logout` | 204 | 1 |
| 5 | `POST /auth/forgot-password` | 204 (sempre igual, exista ou não o e-mail) | 1 |
| 6 | `POST /auth/reset-password` | 204 | 1 |
| 7 | `POST /auth/confirm-email` | 204 | 1 |
| 8 | `GET /users?page&pageSize&search&status` | `PagedResponse<UserApiItem>` | 2 |
| 9 | `GET /users/{id}` | `UserApiItem` | 2 |
| 10 | `POST /users/{id}/activate` | `UserApiItem` | 2 |
| 11 | `POST /users/{id}/deactivate` | `UserApiItem` | 2 |
| 12 | `GET /company-profile` | `CompanyProfileApiItem` — **404 é estado normal** (AD-032) | 3 |
| 13 | `PUT /company-profile` | `CompanyProfileApiItem` | 3 |
| 14 | `GET /offerings` | `OfferingApiItem[]` | 3 |
| 15 | `POST /offerings` | `OfferingApiItem` (201) | 3 |
| 16 | `PUT /offerings/{id}` | `OfferingApiItem` | 3 |
| 17 | `GET /opportunities?page&pageSize&sort&search&state&modality&minValueCents&maxValueCents` | `PagedResponse<OpportunityApiItem>` | 4 |
| 18 | `GET /opportunities/{id}` | `OpportunityDetailApiItem` — 404 é erro real | 4 |
| 19 | `GET /saved-opportunities` | `SavedOpportunityApiItem[]` | 5 |
| 20 | `POST /saved-opportunities` `{opportunityId}` | `SavedOpportunityApiItem` | 5 |
| 21 | `DELETE /saved-opportunities/{opportunityId}` | 204 | 5 |
| 22 | `GET /settings` | `SettingsApiItem` | 5 |
| 23 | `PUT /settings` | `SettingsApiItem` | 5 |
| 24 | `GET /subscription` | `SubscriptionApiItem` | 5 |
| 25 | `GET /gfe/settings` | `CoverageSettingsApiItem` | 6 |
| 26 | `PUT /gfe/settings` | `CoverageSettingsApiItem` | 6 |
| 27 | `GET /gfe/collections?page&pageSize` | `CollectionRunsResponse` (paginação **+** `lastSuccessfulRunAt`) | 6 |

Detalhes que mudam o modelo de dados e passam despercebidos:

- `state` e `modality` em `/opportunities` são **multi-valor** (`?state=SP&state=RJ`),
  `build-opportunities-query.helper.ts:38-46`.
- `sort` aceita só `score | deadline | publishedAt`; `pageSize` é limitado a **100** no cliente —
  limite o mesmo no servidor, não confie no cliente.
- Dinheiro trafega em **centavos, inteiro** (`estimatedValueCents`, `minValueCents`,
  `unitValueCents`). Nunca `decimal` serializado como float.
- `compatibility.score` é `number | null` e a **faixa** (`high`/`medium`/`low`/`poor`/`unrated`) é
  derivada no cliente — o servidor manda o número, nunca a faixa (`opportunity.model.ts:1-7`).
- `SavedOpportunityApiItem` embute o `OpportunityApiItem` inteiro, não só o id.

### Telas prontas sem contrato — precisam de endpoint novo

Servidas hoje por `frontend/mocados/`. São feature de produto existente, não protótipo:

| Área | Telas | Observação |
| --- | --- | --- |
| **Participação assistida** | 7 telas CFE (`list`, `summary`, `requirements`, `proposal`, `review`, `submission`, `history`) + 2 GFE (`operations`, `portals`) | Tem spec própria de backend: `03-participacao-assistida-especificacao-codex.md`, 28 rotas no §15. **É o maior bloco do projeto.** |
| **Cofre documental** | `/cfe/documents` | Upload, versões imutáveis, scanner, download autorizado (§9 e §15 da mesma spec) |
| Painéis e apoio | `/cfe/dashboard`, `/cfe/alerts`, `/cfe/activity`, `/cfe/team`, `/cfe/security`, `/cfe/support`, `/cfe/calendar`, `/cfe/reports`, `/cfe/exports`, `/cfe/onboarding` | Derivam de dados das outras features; contrato a escrever |
| GFE | `/gfe/dashboard` | Indicadores agregados |

### Autorização que o backend precisa reproduzir

Duas áreas (`client` → `/cfe`, `manager` → `/gfe`), resolvidas por `AuthUser.area`. Permissões hoje
declaradas no cliente (`core/permissions/permission.model.ts`):
`users.read`, `users.write`, `collections.read`, `participation-operations.read`,
`participation-portals.manage` — mais as 11 da spec de participação (§16).

**Guard de frontend não é autorização** (SEC-07, SEC-29): cada endpoint revalida área e permissão.

---

## 2. Decisões a tomar antes de escrever código

Sem elas, a primeira feature escolhe por omissão e o resto herda.

| ID | Decisão | Recomendação |
| --- | --- | --- |
| **D-01** | **Um `AppDbContext` ou um por módulo?** A spec de participação (§4) manda monólito modular com *schema/DbContext por módulo* e proíbe join entre schemas. O template tem um `AppDbContext` só. | **Um `DbContext` por módulo, schema PostgreSQL por módulo, desde a Fase 0.** Reverter depois custa migração de dados. Cada módulo vira uma pasta em `Core`/`UseCases`/`Infrastructure`, não um projeto novo. |
| **D-02** | **Multi-tenant: quando entra o `OrganizationId`?** A spec exige ele em toda tabela privada, com filtro explícito na query. | **Na Fase 0**, junto da primeira tabela. Adicionar coluna de tenant depois é migração perigosa em tabela com dado. |
| **D-03** | **Identidade: ASP.NET Core Identity ou implementação própria?** | **Identity com cookie** (`AddIdentityCore` + `AddAuthentication().AddCookie`): entrega hash de senha, tokens de reset/confirmação com expiração e uso único, e lockout — que é exatamente o `DF-004` em aberto no `licitaledital-api`. Custa aceitar o schema dele num schema `identity` próprio. |
| **D-04** | **Tipo de Id exposto:** `int` (como o template) ou opaco? A spec §6 exige *IDs privados opacos*, e todo `*-api.model.ts` tipa `id` como `string`. | **`Guid` v7** (`Guid.CreateVersion7()`, nativo no .NET 9+): ordenável por tempo, não vaza volume, serializa como string. |
| **D-05** | **`/api` ou `/api/v1`?** O frontend chama `/api`; a spec de participação pede `/api/v1`. | **Servir `/api` agora** e tratar versionamento quando houver quebra real — mudar o `apiBaseUrl` do Angular hoje é 1 linha, mas quebra os 3 environments sem ganho. Registre como desvio consciente da spec §15. |
| **D-06** | **De onde vêm as oportunidades?** O PNCP/Compras publica dados abertos; o "Painel de coletas" da GFE pressupõe um worker de ingestão. | **Projeto `LicitaEdital.Worker` separado** (spec §4 pede API e Worker separados), entrando na Fase 6. Até lá, seed manual. |
| **D-07** | **Onde mora o motor de compatibilidade** (score oportunidade × oferta)? | **Serviço de domínio no Core**, calculado na ingestão e persistido — não em tempo de consulta. A ordenação por `sort=score` precisa dele indexado. |

---

## 3. Fase 0 — Fundação

**Objetivo:** o esqueleto compila, sobe contra PostgreSQL e tem as convenções travadas.

### 0.1 Trocar SQL Server + SQLite por PostgreSQL

| Arquivo | Mudança |
| --- | --- |
| `Directory.Packages.props` | Remover `Microsoft.EntityFrameworkCore.SqlServer`, `Microsoft.EntityFrameworkCore.Sqlite`, `SQLite`, `Testcontainers.MsSql`. Adicionar `Npgsql.EntityFrameworkCore.PostgreSQL`, `Testcontainers.PostgreSql`, `Aspire.Hosting.PostgreSQL` |
| `LicitaEdital.Infrastructure.csproj` | Trocar as duas `PackageReference` de provider por `Npgsql.EntityFrameworkCore.PostgreSQL` |
| `InfrastructureServiceExtensions.cs` | Cai toda a cascata `isWindows`/`forceSqlServer`/fallback: uma connection string, `options.UseNpgsql(...)` |
| `MiddlewareConfig.cs` | Cai o ramo `context.Database.IsSqlite()` → `EnsureCreated`. Com PostgreSQL é sempre `MigrateAsync()` |
| `AppHost.cs` | `builder.AddSqlServer("sqlserver")` → `builder.AddPostgres("postgres")`; mantém `.WithLifetime(Persistent)` e o Papercut |
| `appsettings*.json` | Uma `ConnectionStrings:licitaedital` (`Host=localhost;Port=5432;Database=licitaedital;...`). Senha via *user secrets*, nunca no arquivo |
| `CustomWebApplicationFactory.cs` | `MsSqlBuilder` → `PostgreSqlBuilder`; **cai o fallback para SQLite** — sem Docker, o teste funcional falha, e isso é honesto |
| `BaseEfRepoTestFixture.cs` | Manter InMemory **só** para teste que não toca SQL. Mapeamento de `jsonb`, `citext` e índice não se verifica em InMemory — esses vão para os funcionais |

### 0.2 Convenções que o PostgreSQL impõe

- **`snake_case` nas tabelas e colunas.** PostgreSQL dobra identificador não-citado para minúsculo;
  `PascalCase` só sobrevive entre aspas e torna todo SQL manual hostil. Aplicar uma convenção
  global no `OnModelCreating` do módulo.
- **`timestamptz` para todo instante**, `DateTimeOffset`/`DateTime` em UTC via `TimeProvider`
  (spec §6). Data civil sem hora → `DateOnly` → `date`.
- **Dinheiro em `bigint` de centavos**, batendo com o contrato (`*ValueCents`). Nunca `float`.
- **Coleção de string (`positiveKeywords`, `servedRegions`, `catalogCodes`) → `text[]` nativo**, com
  índice GIN quando houver busca. Serializar como JSON string mata o filtro.
- **Concorrência otimista com `xmin`** (`UseXminConcurrencyToken`), que a spec §6 exige em toda
  tabela mutável e que o PostgreSQL já mantém de graça.
- **Busca textual (`?search`)**: `citext` ou `ILIKE` com índice `pg_trgm` na Fase 4; `tsvector`
  quando o volume pedir. Decidir com dado, não agora.

### 0.3 Estrutura de módulos (D-01)

```
src/LicitaEdital.Core/
  Identity/            usuário, área, permissão, organização, membership
  Companies/           perfil da empresa
  Catalog/             oportunidade, modalidade, item, documento publicado
  Offerings/           oferta do cliente, termos, compatibilidade
  Engagement/          salvas, alertas, preferências, assinatura
  Documents/           cofre (Fase 7)
  Participations/      participação assistida (Fase 8)
  Collections/         execuções de coleta
```

Mesmo recorte em `UseCases/` e `Infrastructure/Data/<Modulo>/`. Um schema PostgreSQL por módulo
(`identity`, `catalog`, …). Referência entre módulos é **id opaco + contrato de leitura**, nunca FK
entre schemas.

### 0.4 Transversais que nascem aqui

- **ProblemDetails com `code` e `traceId`** em toda resposta de erro (spec §15). Um handler de
  exceção do FastEndpoints, não `try/catch` por endpoint.
- **CORS restrito** à origem do Angular (SEC-44, SEC-45) — nunca `*` em API com cookie.
- **Antiforgery** emitindo `XSRF-TOKEN` e validando `X-XSRF-TOKEN` (SEC-46).
- **Security headers** e HSTS (SEC-61, SEC-13).
- **Rate limiting** nos endpoints de auth (SEC-65).
- **Serilog + OpenTelemetry** já vêm do template; enriquecer com o `X-Correlation-ID` recebido.

**Pronto quando:** `dotnet run --project src/LicitaEdital.AspireHost` sobe PostgreSQL em container,
a API aplica migração numa base vazia e `GET /api/health` responde 200 com o cookie XSRF setado.

---

## 4. Fase 1 — Autenticação

Fecha `FEAT-12.1`..`FEAT-12.8` e os 9 `AUTH-NN` de backend. Requisito completo em
`licitaledital-api`: `docs/features/auth.md`.

**Endpoints:** 1 a 7 da tabela.

**Domínio:** `User` (e-mail, hash, displayName, área, status), `Organization`, `Membership`,
`Permission`. `AuthUser.permissions` é resolvido da role, **nunca aceito do cliente** (SEC-08).

**Os três pontos que costumam sair errados:**

1. **`forgot-password` não pode revelar se o e-mail existe** — mesmo status, mesmo corpo e **mesmo
   tempo de resposta** para os dois casos. É `DF-004` do `licitaledital-api`, sem regra `SEC-NN`
   própria. Enfileire o e-mail de forma assíncrona para não vazar a diferença por latência.
2. **Token de reset e de confirmação: expiração e uso único.** Segundo item do `DF-004`.
3. **401 em toda rota autenticada**, não só `/auth/me` (`AUTH-12`). O `error.interceptor` do Angular
   depende disso para deslogar.

`register` autentica **sem exigir confirmação de e-mail** — decisão de produto (`FEAT-12.2`,
`AUTH-04`), não descuido.

**Pronto quando:** `authBypassEnabled: false` no `environment.development.ts` e os 5 fluxos de
identificação funcionam contra a API real. É o critério de sucesso de `AD-014` do frontend.

---

## 5. Fase 2 — Organização, usuários e área GFE

**Endpoints:** 8 a 11.

**Domínio:** membership por organização, ativação/desativação, role e permissão efetiva.

`UserApiItem` carrega `lastLoginAt` e `companyName` opcionais — dois campos que só existem se a
Fase 1 registrar login e a Fase 3 vincular empresa. Enquanto não existirem, devolva `null`
explicitamente, não omita o campo.

**Isolamento multi-tenant entra pra valer aqui:** toda query filtra `OrganizationId` de forma
explícita; global query filter é camada adicional, não a principal (spec §16). Recurso de outro
tenant retorna **404**, não 403.

---

## 6. Fase 3 — Empresa e ofertas

**Endpoints:** 12 a 16.

- `GET /company-profile` devolve **404 quando ainda não há cadastro** — o frontend trata isso como
  sucesso com dado `null` e abre formulário vazio (`AD-032`). Não é erro, não logue como erro.
- `PUT` é upsert idempotente.
- `Offering` tem `positiveKeywords`, `negativeKeywords`, `synonyms`, `catalogCodes`,
  `servedRegions` — todos `text[]`, e são a **entrada do motor de compatibilidade** da Fase 4.
- `supplyType` é `product | service | both`: SmartEnum no domínio, string no contrato.

---

## 7. Fase 4 — Oportunidades e compatibilidade

O coração do produto.

**Endpoints:** 17 e 18.

**Domínio:** `Opportunity` (agregado), `OpportunityLineItem`, `OpportunityDocument`,
`Modality` (código + rótulo), `OpportunityStatus`
(`receiving_proposals | closed | cancelled | reopened`).

**Motor de compatibilidade (D-07):** casa termos da `Offering` com o objeto da `Opportunity` e
produz `score`, `offeringId`, `matchedTerms`, `positiveReasons`, `attentionPoints`. Calculado na
ingestão e **persistido por par (oportunidade, organização)** — `sort=score` precisa de índice, e
recalcular por request não escala.

**Consulta:** filtro multi-valor por estado e modalidade, faixa de valor em centavos, busca textual,
três ordenações. É a única listagem do sistema com volume real — candidata natural a query service
com SQL próprio (`IListOpportunitiesQueryService`), fora do repositório genérico.

---

## 8. Fase 5 — Salvas, preferências e assinatura

**Endpoints:** 19 a 24. Baixo risco, alto ganho de tela.

- `saved-opportunities` devolve a **oportunidade inteira** embutida, não só o id.
- `DELETE /saved-opportunities/{opportunityId}` usa o id da **oportunidade**, não o do registro de
  salvamento.
- `/settings`: tipos de alerta, frequência (`immediate | daily-digest | weekly-digest`) e filtro
  padrão. `valueRange` é chave simbólica (`up-to-100k`…), convertida para centavos **no cliente**
  (`build-opportunities-query.helper.ts:11-17`) — guarde a chave, não os centavos.
- `/subscription` hoje devolve só `{ planId }`. Não invente campo que a tela não usa (SEC-64).

---

## 9. Fase 6 — Coleta e cobertura

**Endpoints:** 25 a 27, mais o worker.

- `LicitaEdital.Worker` (projeto novo, spec §4): busca as fontes, normaliza, grava `Opportunity` e
  dispara o recálculo de compatibilidade.
- `CollectionRun` registra `startedAt`, `endedAt`, `newOpportunitiesCount`,
  `result: success | partial | failure`, `errorMessage`.
- `CollectionRunsResponse` é paginação **mais** `lastSuccessfulRunAt` — envelope próprio, não o
  `PagedResponse<T>` padrão.
- Idempotência por referência externa da fonte: reprocessar a mesma coleta não duplica oportunidade.

---

## 10. Fase 7 — Cofre documental

Primeira feature sem contrato firmado. Spec: `03-participacao-assistida...` §9 e §15.

Upload em sessão autorizada → scanner → versão **imutável** com hash → download por link temporário.
Bytes em object storage com chave privada, **nunca servidos direto do banco nem por URL adivinhável**
(§16: bloquear loopback, rede privada e metadata endpoint em qualquer download feito pelo servidor).

Permissões próprias: `company-documents.read | manage | download`.

---

## 11. Fase 8 — Participação assistida

O maior bloco. Tem spec de implementação completa — **leia `03-participacao-assistida-especificacao-codex.md`
antes de começar**, ela é mais detalhada que este plano.

Resumo do que ela fixa e não se negocia:

- **28 rotas** sob `/participations` e `/company-documents` (§15).
- **Três dimensões de estado separadas** — preparação, registro de envio e resultado. Uma coluna só
  seria errado: "Perdida" não é o oposto de "Em análise" (§7).
- **`ManualHandoff` é o modo de produção inicial.** Não existe API oficial autorizada de submissão.
  **Não criar `/submit` nem `/auto-bid`** (§3, §15).
- Proposta com **decimal escalado**, revisões imutáveis, manifesto com hash, aprovação que se
  invalida sozinha quando o conteúdo muda (§10, §11, §12).
- Constraints obrigatórias: unique `(OrganizationId, CompanyId, TenderId)`,
  `(OrganizationId, ProposalId, RevisionNumber)`, Outbox/Inbox com `(ConsumerName, EventId)` (§6).
- Códigos de erro nomeados: `DOCUMENT_NOT_RELEASED`, `REQUIREMENT_UNREVIEWED`, `VERSION_CONFLICT`,
  `APPROVAL_OBSOLETE`… (§15).
- 11 permissões próprias (§16).

Ordem interna: persistência → cofre → participação e checklist → proposta → aprovação e pacote →
envio assistido e histórico → operação GFE (§21).

---

## 12. Fase 9 — Telas de apoio

Dashboard CFE e GFE, alertas, log de atividades, equipe, segurança da conta, suporte, calendário de
prazos, relatórios e exportações. Quase todas são **projeção** do que as fases anteriores já
persistiram — o contrato sai depois que a fonte existe, não antes.

---

## 13. Ordem e dependências

```
Fase 0 Fundação
  └─ Fase 1 Auth
       └─ Fase 2 Organização e usuários
            ├─ Fase 3 Empresa e ofertas ──┐
            │                             ├─ Fase 4 Oportunidades e compatibilidade
            │                             │    ├─ Fase 5 Salvas, preferências, assinatura
            │                             │    └─ Fase 6 Coleta e cobertura
            └─ Fase 7 Cofre ──────────────┴─ Fase 8 Participação assistida
                                                └─ Fase 9 Telas de apoio
```

As fases 3 e 7 podem correr em paralelo com a 4; a 8 depende das duas.

---

## 14. Riscos registrados

| Risco | Efeito | Mitigação |
| --- | --- | --- |
| `docs/shared/` (contratos canônicos) não abre desta máquina | O contrato usado aqui foi reconstruído dos modelos TypeScript do frontend — fiel por `check-contract.mjs`, mas sem os `§` de erro e paginação escritos em prosa | Conseguir acesso a `Cez4rRxvxl/licitaedital-docs` antes da Fase 1 |
| Este repositório é git isolado, sem a esteira SDD nem as 21 regras `SEC-NN` | Regra de segurança citada por ID não resolve para texto nenhum daqui | Trazer `docs/rules/seguranca-backend.md` do `licitaledital-api`, ou reconciliar os dois repositórios |
| `AD-002` do frontend descartou OpenAPI e contract testing | Divergência front/back só aparece em runtime | A rede prevista é `FEAT-NN` reivindicado dos dois lados (`AD-013`). Declare no início de cada fase quais IDs ela cobre |
| Fonte das oportunidades não está decidida (D-06) | Fase 4 entrega consulta sobre base vazia | Resolver junto da Fase 3; até lá, seed representativo derivado de `frontend/mocados/` |
| A spec de participação pede `DbContext` por módulo; o template dá um só | Refatorar depois custa migração de dados | **D-01 na Fase 0**, antes da primeira tabela |

---

## 15. Antes de começar a Fase 0

1. Fechar **D-01 a D-07** — são sete decisões, cada uma com recomendação escrita acima.
2. Conseguir acesso ao `licitaedital-docs` (contratos canônicos) e ao `licitaledital-api`
   (regras `SEC-NN`, fatia de auth).
3. Confirmar se este repositório segue isolado ou reconcilia com o `licitaledital-api` — muda onde
   a spec de cada fase vai morar.
