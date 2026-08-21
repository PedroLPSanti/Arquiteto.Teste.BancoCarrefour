# Carrefour.API.BusinessIntelligence.Tests

Projeto de testes automatizados: **`Carrefour.API.BusinessIntelligence.Tests.csproj`**

Contém os testes automatizados da API de Business Intelligence (Controller, Services e Repository).

## Stack de testes

- **xUnit** — framework de testes
- **Moq** — mocking de dependências (`IDailyConsolidatedService`, `IDailyConsolidatedRepository`, `IDistributedCache`)
- **FluentAssertions** — assertions mais legíveis
- **EF Core InMemory** — banco de dados em memória para testar o repositório sem depender do Postgres real
- **FrameworkReference** (`Microsoft.AspNetCore.App`) — necessário para usar tipos de `Microsoft.AspNetCore.Mvc` (como `OkObjectResult`) nos testes de controller

## Como rodar os testes
### Restaurar dependências

```bash
dotnet restore Carrefour.API.BusinessIntelligence.Tests.csproj
```

### Rodar todos os testes

```bash
dotnet test Carrefour.API.BusinessIntelligence.Tests.csproj
```

### Rodar apenas uma classe de teste

```bash
dotnet test --filter "FullyQualifiedName~CachedDailyConsolidatedServiceTests"
```

### Rodar apenas um método específico

```bash
dotnet test --filter "FullyQualifiedName~DailyConsolidatedRepositoryTests.ReadAllAsync_WhenRecordsExist_ReturnsAllRecords"
```

### Rodar com relatório de cobertura (opcional)

```bash
dotnet test --collect:"XPlat Code Coverage"
```

## Resumo dos testes

### Controller — `DailyConsolidatedControllerTests`

| Teste | O que valida |
|---|---|
| `GetAll_WhenCalled_ReturnsOkWithListOfConsolidatedData` | O `GetAll` retorna `200 OK` com a lista exata devolvida pelo service; o service é chamado uma única vez com o `CancellationToken` correto |
| `GetAll_WhenNoDataExists_ReturnsOkWithEmptyList` | O `GetAll` continua retornando `200 OK` (e não erro) quando o service devolve uma lista vazia |

### Service — `DailyConsolidatedServiceTests`

Camada sem cache: apenas mapeia os dados vindos do repository para DTO.

| Teste | O que valida |
|---|---|
| `ReadAllAsync_WhenRepositoryReturnsItems_MapsAllToDtos` | Cada `DailyConsolidated` retornado pelo repository é corretamente convertido em `DailyConsolidatedDTO`, incluindo o cálculo de `valueTotal`; usa dois registros com resultado positivo e negativo para pegar uma eventual troca de `valueCredit`/`valueDebit` |
| `ReadAllAsync_WhenRepositoryReturnsEmpty_ReturnsEmptyList` | Retorna lista vazia (e não `null`) quando o repository não tem dados |
| `ReadAllAsync_PassesCancellationTokenToRepository` | O `CancellationToken` recebido é repassado corretamente ao repository |

### Service (decorator de cache) — `CachedDailyConsolidatedServiceTests`

Camada que envolve `IDailyConsolidatedService` adicionando cache distribuído (`IDistributedCache`) por cima.

| Teste | O que valida |
|---|---|
| `ReadAllAsync_WhenCacheHasData_ReturnsCachedData_AndDoesNotCallInnerService` | Quando há dado em cache, ele é retornado diretamente e o service interno **não** é chamado — comprova o short-circuit |
| `ReadAllAsync_WhenCacheIsEmpty_CallsInnerService_AndReturnsItsData` | Quando o cache está vazio, o service interno é chamado e seu resultado é retornado sem transformação adicional |
| `ReadAllAsync_WhenCacheIsEmpty_AndInnerServiceHasData_StoresResultInCache` | O resultado do service interno é serializado e salvo no cache com o tempo de expiração correto (1 minuto) |
| `ReadAllAsync_WhenCacheIsEmpty_AndInnerServiceReturnsEmptyList_DoesNotWriteToCache` | Uma lista vazia **não** é gravada no cache, evitando mascarar dados reais por até 1 minuto |
| `ReadAllAsync_WhenCachedJsonDeserializesToNull_FallsBackToInnerService` | Um valor de cache presente mas que desserializa para `null` (ex: string `"null"`) cai corretamente para o service interno |
| `ReadAllAsync_PassesCancellationTokenToCacheAndInnerService` | O `CancellationToken` é repassado corretamente tanto para o cache quanto para o service interno |

### Repository — `DailyConsolidatedRepositoryTests`

| Teste | O que valida |
|---|---|
| `ReadAllAsync_WhenRecordsExist_ReturnsAllRecords` | Todos os registros persistidos são retornados com os valores corretos |
| `ReadAllAsync_WhenNoRecordsExist_ReturnsEmptyList` | Retorna vazio (e não `null`) quando não há registros |
| `ReadAllAsync_ReturnsUntrackedEntities` | Confirma que `.AsNoTracking()` está de fato sendo aplicado — protege contra regressão silenciosa de performance |

---

## Estrutura geral de cobertura

```
Controller                       → 2 testes  (mapeamento HTTP, lista vazia)
Service (sem cache)              → 3 testes  (mapeamento repo → DTO, lista vazia, CancellationToken)
Service (decorator de cache)     → 6 testes  (cache hit/miss, TTL, não cachear vazio, edge case de JSON nulo, CancellationToken)
Repository                       → 3 testes  (persistência, lista vazia, AsNoTracking)
─────────────────────────────────────────
Total: ~14 testes
```

## Observação sobre a arquitetura

Diferente do projeto `Carrefour.API.Ledger`, este projeto **não segue um fluxo linear simples** `Controller → Service → Repository`. Aqui existe um **decorator de cache**:

```
Controller → CachedDailyConsolidatedService → DailyConsolidatedService → Repository
                  (implementa IDailyConsolidatedService)   (implementa IDailyConsolidatedService)
```

Ambas as classes implementam a mesma interface `IDailyConsolidatedService`. Isso só funciona corretamente se a injeção de dependência (DI) estiver configurada para que o `CachedDailyConsolidatedService` seja a camada externa, injetando `DailyConsolidatedService` como `innerService`. Um erro de configuração no DI (ex: registrar as duas diretamente como `IDailyConsolidatedService`, fazendo o último registro "vencer") pode fazer o cache nunca ser aplicado — esse tipo de problema **não é pego pelos testes unitários** (já que cada classe é testada isoladamente) e é um bom candidato para um teste de integração futuro (`WebApplicationFactory`, resolvendo `IDailyConsolidatedService` do container e verificando o tipo concreto retornado).