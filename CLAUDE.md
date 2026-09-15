# LicitaEdital — API

Backend do **licitaedital** (Radar Inteligente de Licitações). .NET 10, Clean Architecture sobre o
template [ardalis/CleanArchitecture](https://github.com/ardalis/CleanArchitecture), com o código de
exemplo do template já removido.

Este arquivo é a instrução única para qualquer agente que trabalhe aqui. Regras invioláveis
primeiro, padrão por camada depois.

---

## Regras invioláveis

1. **A regra de dependência não se quebra.** `Core` não referencia ninguém. `UseCases` referencia
   só `Core`. `Infrastructure` e `Web` referenciam para dentro, nunca o contrário. Se uma
   implementação exige que `Core` conheça EF Core, HTTP, e-mail ou qualquer detalhe externo, a
   resposta é **uma interface em `Core` implementada na camada de fora** — nunca uma referência nova
   no `.csproj` do `Core`.
2. **Warning é erro.** `TreatWarningsAsErrors` está ligado em `Directory.Build.props`. Não silencie
   com `#pragma` nem com `NoWarn` novo: corrija. `Nullable` está habilitado — `!` só com motivo
   escrito ao lado.
3. **Versão de pacote não vai no `.csproj`.** Central Package Management está ligado: `<PackageReference
   Include="X" />` sem `Version`, e a versão entra em `Directory.Packages.props`. Pacote novo é
   decisão, não detalhe — justifique antes de adicionar.
4. **Uma implementação, um lugar.** Antes de criar helper, extensão ou abstração, procure primeiro
   na lib — `ResultExtensions`, `ApiProblem`, `PagedList`, `PageRequest`, `ErrorCodes`,
   `EfRepository`, `Cnpj`, `StateCode` moram lá — e depois em `Core/Interfaces/` e
   `Infrastructure/Data/Config/`.
5. **Migração de banco é arquivo gerado, nunca escrito à mão.** Sempre via `dotnet ef migrations
   add` (comando exato na seção *Comandos*), e sempre revisada antes de aceitar.
6. **A base vem da lib, nunca reescrita aqui.** Entidade, evento de domínio, repositório, CQRS,
   autenticação, interceptors de persistência e ProblemDetails moram em
   `LicitaEdital.BuildingBlocks` (repositório irmão, consumido por pacote). Antes de escrever
   qualquer classe base, procure lá. Se falta algo, o lugar de acrescentar é a lib — **desde que o
   tipo não conheça o produto**; se ele precisa da palavra "licitação", é daqui.
7. **Não commitar.** Commit é papel do dev. Também não compile para validar — o dev faz isso.

---

## Estrutura

```
src/
  LicitaEdital.Core/            domínio: entidades, agregados, value objects, eventos,
                                specifications, interfaces. Zero dependência de framework.
  LicitaEdital.UseCases/        CQRS: commands, queries, handlers, DTOs. Referencia só o Core.
  LicitaEdital.Infrastructure/  escrita: DbContext por módulo, mapeamento, repositórios.
  LicitaEdital.Query/           leitura: contexto sem rastreamento e query services.
  LicitaEdital.Facade/          contratos de leitura entre módulos.
  LicitaEdital.Providers/       integração com sistema externo, um diretório por sistema.
  LicitaEdital.Web/             FastEndpoints (REPR), validação de request, composição de DI.
  LicitaEdital.AspireHost/      orquestração local: PostgreSQL + Papercut (SMTP de teste).
  LicitaEdital.ServiceDefaults/ OpenTelemetry, health checks, service discovery, resiliência.
tests/
  LicitaEdital.UnitTests/         domínio e handlers, sem I/O
  LicitaEdital.IntegrationTests/  banco e componentes de Infrastructure
  LicitaEdital.FunctionalTests/   endpoints HTTP ponta a ponta
```

Dois caminhos, e eles não se misturam:

```
escrita   Endpoint → IMediator.Send(Command) → Handler → IRepository<T> → Infrastructure → banco
leitura   Endpoint → IMediator.Send(Query)   → Handler → IXQueryService → Query → banco (sem tracking)
```

A fronteira entre módulos é atravessada por **fachada**, nunca por `DbContext` de outro módulo nem
por join entre schemas.

---

## Convenções de C#

Governadas por `.editorconfig` — ele é a fonte, o que segue é o resumo do que mais aparece:

- **Indentação de 2 espaços**, arquivos `.cs` em **UTF-8 com BOM**, newline final.
- **`namespace` file-scoped** (`namespace X;`), `using` **fora** do namespace, `System` primeiro.
- `PascalCase` para tipos e membros, `camelCase` para parâmetros, `I` em interface, sufixo `Async`
  em método assíncrono, `_` em campo privado.
- **Primary constructors** para dependências. No template o parâmetro às vezes é atribuído a um
  campo `private readonly` e às vezes é nomeado `_repository` direto no construtor — **padronize no
  campo**: `public class Foo(IRepository<Bar> repository) { private readonly IRepository<Bar>
  _repository = repository; }`. Nunca use o parâmetro do primary constructor diretamente no corpo.
- Chaves sempre, **exceto** saída de uma linha: `if (x == null) return Result.NotFound();` fica numa
  linha só.
- `var` liberado; propriedade pode ser expression-bodied, método não.

---

## Padrão por camada

### Core — domínio

Um diretório por **módulo**, e dentro dele um por agregado:
`src/LicitaEdital.Core/<Modulo>/<Nome>Aggregate/`, com `Events/`, `Handlers/` e `Specifications/`
dentro quando houver. Os módulos são `Identity`, `Companies`, `Catalog`, `Offerings`, `Engagement`,
`Collections` — e `Shared`, só para os ids que atravessam módulo.

**Entidade / raiz de agregado** — herda de uma das três bases da lib. Setter é privado; mudança de
estado passa por método que carrega a intenção e registra o evento de domínio:

| Base | Dá | Use quando |
| --- | --- | --- |
| `Entity<TId>` | id UUID v7 automático, eventos, igualdade | filho de agregado |
| `AuditableEntity<TId>` | + `CreatedAt/By`, `UpdatedAt/By` | raiz que **não** deve ter soft delete — projeção, registro de execução, alternador |
| `AggregateRoot<TId>` | + `IAggregateRoot`, `IsActive`, `DeletedAt/By` | o default de uma raiz |

```csharp
public class Company : AggregateRoot<CompanyId>, ITenantScoped
{
  private Company(CompanyName name) { Name = name; }

  public CompanyName Name { get; private set; }
  public OrganizationId OrganizationId { get; private set; }

  Guid ITenantScoped.TenantId => OrganizationId.Value;

  public static Company Create(CompanyName name) => new(name);

  public Company UpdateName(CompanyName newName)
  {
    if (Name == newName) return this;
    Name = newName;
    RegisterDomainEvent(new CompanyNameUpdatedEvent(Id));
    return this;
  }
}
```

**Nunca redeclare `Id`, `CreatedAt`, `UpdatedAt` nem `IsActive`** — vêm da base, e uma cópia local
seria a segunda fonte da mesma verdade. Factory **não recebe `TimeProvider`** só para carimbar data:
o interceptor de auditoria faz isso. `TimeProvider` só entra quando a data é fato de negócio
(`CollectedAt`, `StartedAt`, `CalculatedAt`).

**Documento e localidade brasileiros vêm da lib.** `Cnpj` (numérico **e** alfanumérico) e
`StateCode` (as 27 UFs) estão em `LicitaEdital.BuildingBlocks.Brasil` — não reimplemente, e ponha lá
o próximo do mesmo tipo (CPF, CEP, inscrição estadual).

**Value object e Id do produto** — `Vogen`, com validação no próprio tipo. A configuração global do Vogen
(`[assembly: VogenDefaults]`) mora em `Core/VogenConfiguration.cs`; não a duplique. Todo id declara
`IGuidId<TSelf>`, que é o contrato pelo qual a base gera o valor:

```csharp
[ValueObject<Guid>]
public readonly partial struct CompanyId : IGuidId<CompanyId>
{
  private static Validation Validate(Guid value)
      => value != Guid.Empty ? Validation.Ok : Validation.Invalid("CompanyId nao pode ser vazio.");
}
```

Id é `Guid` v7 **gerado no construtor da entidade** (D-04), nunca pelo banco: ordenável por tempo,
não vaza volume, e o mapeamento usa `ValueGeneratedNever()`. Nenhuma factory chama `New()`.

**Enum de domínio** — `Ardalis.SmartEnum`, não `enum` nativo, quando o valor tem comportamento ou
precisa persistir estável.

**Evento de domínio** — `record` herdando `DomainEvent` (da lib), disparado por
`RegisterDomainEvent` na entidade (ou `IMediator.Publish` num serviço de domínio, quando não há entidade viva — caso de
delete). O despacho acontece depois do `SaveChanges`, em
`Infrastructure/Data/EventDispatcherInterceptor.cs`. **Handler de evento nunca recebe `DbContext`.**

**Specification** — `Ardalis.Specification`, em `Specifications/`, uma classe por consulta do
domínio. É o único jeito de filtrar por `IRepository<T>`.

**Interface** — repositório e serviço externo declarados em `Core/Interfaces/`, implementados fora.
`IEmailSender` é o exemplo vivo.

### UseCases — aplicação

Um diretório por agregado, um subdiretório por operação:
`UseCases/<Agregado>/<Operacao>/` com o command/query e o handler.

```csharp
public record CreateCompanyCommand(CompanyName Name) : ICommand<Result<CompanyId>>;

public class CreateCompanyHandler(IRepository<Company> repository)
  : ICommandHandler<CreateCompanyCommand, Result<CompanyId>>
{
  private readonly IRepository<Company> _repository = repository;

  public async ValueTask<Result<CompanyId>> Handle(CreateCompanyCommand command, CancellationToken ct)
  {
    var created = await _repository.AddAsync(new Company(command.Name), ct);
    return created.Id;
  }
}
```

- **Command** para mutação, **Query** para leitura. Toda operação retorna `Result` / `Result<T>` do
  `Ardalis.Result` — nunca exceção como fluxo de controle, nunca tipo do ASP.NET aqui.
- **Mediator é o source generator** (pacote `Mediator`, de martinothamar), **não MediatR**: handler
  devolve `ValueTask`, e o assembly precisa estar listado em `Web/Configurations/MediatorConfig.cs`.
- **Query pode furar o repositório** por performance: declare a interface do query service e o DTO
  aqui (`IListCompaniesQueryService`), implemente em `LicitaEdital.Query/<Modulo>/`, e devolva DTO.
  Vale quando a consulta filtra, ordena e pagina — trazer o agregado inteiro para descartar em
  memória é o que a exceção evita.
- **DTO fica aqui**, não no `Core` nem no `Web` — a exceção é o DTO de **fachada**, que é contrato
  entre módulos e mora em `Core/<Modulo>/Facade/`. Paginação usa `PagedList<T>` e `PageRequest`
  da lib, que já limita o tamanho de página no servidor.
- Cross-cutting (log, validação, cache) entra como pipeline behavior em `MediatorConfig`, nunca
  espalhado nos handlers.

### Query — o lado de leitura

Um projeto só para consulta. `LicitaEdital.Query`, um diretório por módulo.

- Cada módulo tem um `<Modulo>ReadContext` herdando `ReadOnlyModuleDbContext` da lib. Ele já vem
  **sem rastreamento, sem detecção automática de alteração, sem lazy loading e recusando
  `SaveChanges`** — **nunca escreva `AsNoTracking()`**: o rastreamento não chega a ser ligado.
- O mapeamento é o mesmo do lado de escrita: o read context aponta `ConfigurationAssembly` para
  `LicitaEdital.Infrastructure`. Não redeclare `IEntityTypeConfiguration` aqui — dois mapeamentos
  divergem na primeira coluna renomeada.
- **A interface e o DTO ficam em `UseCases`**, a implementação aqui. O handler não sabe se quem o
  atende é repositório ou query service.
- Listagem paginada usa `ToPagedListAsync(page, ct)` da lib. Filtro opcional usa `WhereIf` — a
  alternativa (`WHERE (@p IS NULL OR coluna = @p)`) costuma cegar o índice.
- Busca textual com `EF.Functions.ILike`, nunca `ToLower().Contains()`, que impede o uso de índice.
- `NULLS LAST` se escreve `OrderBy(x => x.Campo == null).ThenBy...` — sem isso o PostgreSQL põe
  `NULL` primeiro num `DESC`, e o feed abre com as linhas sem nota no topo.
- Referência viva: `Query/Catalog/ListOpportunitiesQueryService.cs`.

### Facade — leitura entre módulos

Um módulo **nunca** alcança o `DbContext`, o schema ou as tabelas de outro (spec §4). O que
atravessa é um contrato de leitura:

- **Contrato e DTO em `Core/<Modulo>/Facade/`**, implementação em `LicitaEdital.Facade/<Modulo>/`.
  O contrato no Core é o que permite ao módulo consumidor enxergar o tipo sem referenciar o projeto
  de fachada.
- Devolve DTO, nunca entidade — entregar o agregado daria ao outro módulo os métodos que o mudam.
- **Recebe coleção de ids, não um id por chamada.** N+1 atravessando fronteira de módulo é o pior
  lugar para ele acontecer.
- O DTO da fachada é **propositalmente separado** do DTO da API, mesmo quando hoje coincidem: um é
  o acordo entre módulos, o outro é o contrato com o cliente, e cada um muda por sua razão.
- Referência viva: `Core/Catalog/Facade/ICatalogFacade.cs` + `Facade/Catalog/CatalogFacade.cs`.

### Providers — sistemas externos

`LicitaEdital.Providers`, um diretório por sistema (`Smtp/`, e a seguir `Pncp/`, `ComprasGov/`,
`Storage/`).

- **O projeto não referencia EF Core, e isso é trava, não acaso.** Sem `DbContext` ao alcance,
  ninguém abre transação em volta de uma chamada HTTP — o que a spec §14 proíbe explicitamente.
- A interface fica em `Core/Interfaces/`; a implementação, aqui.
- Provider HTTP usa `AddExternalHttpClient` da lib, **nunca `AddHttpClient` nu**: é o que traz a
  guarda de destino (SSRF — bloqueia loopback, rede privada, link-local e metadata endpoint,
  revalidando a cada redirect), o bloqueio de redirect automático, a resiliência e o timeout.
- Falha de terceiro é `ProviderException`, que carrega o nome do provedor e **não** carrega corpo de
  resposta nem URL completa — é onde token e credencial costumam estar (§19).

### Infrastructure — dados e integrações

Banco é **PostgreSQL**, com **um `DbContext` e um schema por módulo** (D-01). O mapa completo está
em [`docs/modelagem-dados.md`](docs/modelagem-dados.md).

Só **escrita**. Consulta de leitura é do projeto `LicitaEdital.Query`.

- `Data/<Modulo>/<Modulo>DbContext.cs`: um `DbSet` por raiz de agregado do módulo, `HasDefaultSchema`
  do seu schema, e `ApplyConfigurationsFromAssembly` **filtrado pelo namespace do próprio módulo** —
  sem o filtro, um contexto aplicaria a configuração dos outros cinco.
- Mapeamento em `Data/<Modulo>/Config/<Entidade>Configuration.cs` (`IEntityTypeConfiguration<T>`) —
  **nunca** por atributo na entidade, que sujaria o `Core`.
- Todo value object **do produto** precisa do conversor declarado em
  `Data/Config/VogenEfCoreConverters.cs` (`[EfCoreConverter<T>]`) e de `.HasVogenConversion()` na
  propriedade. **Sem a entrada lá, a coluna simplesmente não é gerada — e não há erro de
  compilação.**
- Value object **da lib** (`Cnpj`, `StateCode`) não passa pelo Vogen: usa
  `.HasCnpjConversion()` / `.HasStateCodeConversion()` de `BrasilValueConverters`.
- **Nada de FK entre schemas.** Referência a outro módulo é o id puro, sem propriedade de navegação.
- Repositório genérico é `EfRepository<TContext, T>` (**da lib**), e cada agregado é registrado
  **fechado** em `InfrastructureServiceExtensions.AddAggregate<TContext, TAggregate>()`. Agregado
  novo sem esse registro falha em runtime, não na compilação.
- `snake_case`, filtro de soft delete e aplicação das configurações do módulo vêm de
  `ModuleDbContext` da lib. Só o `IdentityDbContext` fica de fora, porque precisa da base do ASP.NET
  Identity — ele chama `ApplyModuleConventions` direto.
- Auditoria, soft delete, guarda de tenant e despacho de eventos são **4 interceptors da lib**,
  ligados por `options.AddBuildingBlocksInterceptors(provider)`. Não carimbe data nem autor no
  handler.
- Todo agregado mutável leva `UseXminConcurrencyToken()` da lib.
- **Índice único sobre entidade com soft delete leva `.ActiveOnly()`** — sem o filtro parcial, a
  linha excluída continua ocupando o índice e o recadastro falha contra um registro invisível.
- Nunca vaze tipo de EF Core (`DbContext`, `IQueryable` de entidade) para `UseCases` ou `Web`.

### Web — API

FastEndpoints no padrão **REPR** (Request-Endpoint-Response). Um diretório por recurso, **um arquivo
por operação**: `Create.cs`, `GetById.cs`, `List.cs`, `Update.cs`, `Delete.cs`.

```csharp
public class Create(IMediator mediator)
  : Endpoint<CreateCompanyRequest,
             Results<Created<CreateCompanyResponse>, ValidationProblem, ProblemHttpResult>>
{
  private readonly IMediator _mediator = mediator;

  public override void Configure()
  {
    Post(CreateCompanyRequest.Route);
    Tags("Companies");
    Summary(s => { s.Summary = "..."; s.Description = "..."; });
  }

  public override async Task<Results<Created<CreateCompanyResponse>, ValidationProblem, ProblemHttpResult>>
    ExecuteAsync(CreateCompanyRequest request, CancellationToken ct)
  {
    var result = await _mediator.Send(new CreateCompanyCommand(CompanyName.From(request.Name!)), ct);
    return result.ToCreatedResult(id => $"/Companies/{id}", id => new CreateCompanyResponse(id.Value, request.Name!));
  }
}
```

- Request declara sua rota em `public const string Route`, e o validador é um
  `Validator<TRequest>` (FluentValidation) no mesmo arquivo do endpoint ou em
  `<Operacao>.<Nome>Validator.cs` — o template usa os dois; **para recurso novo, um arquivo por
  tipo** (`Create.cs`, `Create.CreateCompanyRequest.cs`, `Create.CreateCompanyValidator.cs`), que é
  o que reduz conflito de merge.
- **Não converta `Result` na mão.** Use `ResultExtensions`: `ToCreatedResult`, `ToGetByIdResult`,
  `ToUpdateResult`, `ToDeleteResult`, `ToOkOnlyResult`. Faltou um caso? Adicione lá, não no endpoint.
- **`AllowAnonymous()` é decisão de segurança explícita.** Endpoint novo nasce autenticado; abrir
  exige motivo escrito no `Configure()`.
- Registro de serviço vai em `Configurations/` (`ServiceConfigs`, `OptionConfigs`, `MediatorConfig`,
  `MiddlewareConfig`, `LoggerConfigs`). **`Program.cs` só compõe** — não ganha `AddScoped`.
- Endpoint **não fala com repositório nem com `DbContext`**: só com `IMediator`.

---

## Onde validar

Três níveis, com responsabilidades distintas — não duplique, não pule:

| Nível | Onde | O quê |
| --- | --- | --- |
| API | `Validator<TRequest>` (FluentValidation) | forma do input: obrigatório, tamanho, formato |
| Use case | início do handler | pré-condições da operação, existência, autorização de dado |
| Domínio | construtor / método da entidade, `Validate` do Vogen | invariante de negócio — lança exceção, assume input já validado |

---

## Testes

| Projeto | Cobre | Regra |
| --- | --- | --- |
| `UnitTests` | entidade, value object, specification, handler | sem I/O, sem banco. Dublê com `NSubstitute`, `NoOpMediator` para `IMediator` |
| `IntegrationTests` | repositório e interceptor | `BaseEfRepoTestFixture<TContext>` dá um contexto de módulo InMemory. **Não serve para mapeamento, constraint nem concorrência** — InMemory não tem `text[]`, `xmin`, schema nem índice único |
| `FunctionalTests` | endpoint HTTP, mapeamento, constraint | `CustomWebApplicationFactory` sobe PostgreSQL via Testcontainers e roda as migrations. **Sem fallback**: sem Docker, o teste falha — é o ponto |

xUnit v3 + `Shouldly`. Nome do arquivo e da classe seguem `<Sujeito>_<Comportamento>` /
`<Classe><Metodo>` — o que existe no repositório hoje é `DockerAvailabilityTests`; siga o mesmo
tom descritivo.

---

## Comandos

```bash
# build e teste
dotnet build LicitaEdital.slnx
dotnet test  LicitaEdital.slnx --settings .runsettings

# rodar só a API (exige um PostgreSQL alcançável pela connection string)
dotnet run --project src/LicitaEdital.Web

# rodar tudo com Aspire (PostgreSQL + Papercut em container)
dotnet run --project src/LicitaEdital.AspireHost

# migração — a partir de src/LicitaEdital.Web/, UMA POR CONTEXTO.
# Contextos: IdentityDbContext, CompaniesDbContext, CatalogDbContext,
#            OfferingsDbContext, EngagementDbContext, CollectionsDbContext
dotnet ef migrations add <Nome> -c CatalogDbContext \
  -p ../LicitaEdital.Infrastructure/LicitaEdital.Infrastructure.csproj \
  -s LicitaEdital.Web.csproj -o Data/Catalog/Migrations

dotnet ef database update -c CatalogDbContext \
  -p ../LicitaEdital.Infrastructure/LicitaEdital.Infrastructure.csproj \
  -s LicitaEdital.Web.csproj
```

Mudou a lib? O ciclo é: subir `VersionPrefix` lá, `dotnet pack -c Release` (o `.nupkg` cai direto no
feed), atualizar a versão em `Directory.Packages.props` daqui e restaurar.

Em desenvolvimento a API sobe com Scalar em `/scalar` e Swagger em `/swagger`; a lista de serviços
registrados fica em `/listservices`.

---

## O que não fazer

- Referenciar `Infrastructure` a partir de `Core`, `UseCases` ou de um teste unitário.
- Usar `MediatR` (o pacote é `Mediator`), `AutoMapper` (mapeie à mão no handler) ou Controllers
  (`FastEndpoints` é o padrão).
- Colocar regra de negócio em endpoint, em `Program.cs` ou em método de extensão de DI.
- Criar entidade com setter público ou construtor sem invariante.
- Escrever migração à mão, ou apagar/editar migração já aplicada.
- Fazer seed de dado de runtime em git. Semente versionada só para conhecimento genérico de domínio.
- Colocar segredo em `appsettings*.json`. Em desenvolvimento use *user secrets*; em produção, o
  provedor de configuração do ambiente.

---

## Plano de construção

O caminho do esqueleto vazio até a API que sustenta o frontend está em
[`docs/plano-backend.md`](docs/plano-backend.md): os 27 endpoints com contrato firmado, as telas
ainda sem contrato, 7 decisões a fechar antes da primeira linha (`D-01`..`D-07`) e 10 fases com
critério de pronto. **Leia antes de começar qualquer feature.**

O banco é **PostgreSQL**; a troca do template (SQL Server + SQLite) já foi feita.

A modelagem das Fases 1 a 6 está em [`docs/modelagem-dados.md`](docs/modelagem-dados.md): os seis
módulos, um `DbContext` e um schema cada, com o diagrama de cada um, as referências entre módulos e
as convenções de mapeamento. **Não há migration ainda** — as entidades e os mapeamentos existem, o
`dotnet ef migrations add` é o passo seguinte.

## Estado do repositório

Este repositório nasceu do template `ardalis/CleanArchitecture`. Já foram removidos: o agregado de
exemplo `Contributor` inteiro (Core, UseCases, Web, Infrastructure, migrações e testes), o `sample/`
(`NimblePros.SampleToDo`), o `MinimalClean/`, o `.template.config/`, os `.nuspec`, o site de docs
Hugo, os workflows e a documentação do projeto upstream, e o `AspireTests` (estava em `net9.0` e
fora da solution). **Não há código de aplicação ainda** — a primeira feature começa do esqueleto.

Os projetos foram renomeados de `Clean.Architecture.*` para `LicitaEdital.*`, e o banco do Aspire de
`cleanarchitecture` para `licitaedital`.

### Os outros repositórios do projeto

| Repositório | Papel |
| --- | --- |
| `lucasvidalz/licitaedital-web` | frontend Angular |
| `lucasvidalz/licitaledital-api` | backend anterior a este — **só documentação**, sem código: 21 regras de segurança em `docs/rules/seguranca-backend.md`, a fatia de auth em `docs/features/auth.md`, e a esteira SDD (`.specs/`) |
| `Cez4rRxvxl/licitaedital-docs` | material cross-repo: produto, features com ID `FEAT-NN`, contratos de API |

> ⚠️ O nome `licitaledital-api` tem um `l` a mais (`licita` + `l` + `edital`) e é o canônico.
> `licitaedital-api`, sem o `l`, **resolve** — para o repositório do frontend, por um redirect de
> rename. Nunca escreva essa URL por analogia.

Este repositório foi iniciado como git **isolado**, sem remote, por decisão do dono em 14/09/2026.
O contrato de API, os requisitos de produto e as regras de segurança que o backend precisa cumprir
continuam vivos nos repositórios acima — consulte-os antes de especificar uma feature, e traga para
cá o que for escopo de implementação.
