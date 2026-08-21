# Carrefour.API.Ledger.Tests

Projeto de testes automatizados: **`Carrefour.API.Ledger.Tests.csproj`**

Contém os testes automatizados da API de Ledger (Controller, Service, Repository e DTOs/Model).

## Stack de testes

- **xUnit** — framework de testes
- **Moq** — mocking de dependências (`ILedgerActivityService`, `ILedgerActivityRepository`)
- **FluentAssertions** — assertions mais legíveis
- **EF Core InMemory** — banco de dados em memória para testar o repositório sem depender de um banco real

## Como rodar os testes

### Restaurar dependências

```bash
dotnet restore Carrefour.API.Ledger.Tests.csproj
```

### Rodar todos os testes

```bash
dotnet test Carrefour.API.Ledger.Tests.csproj
```

### Rodar apenas uma classe de teste

```bash
dotnet test --filter "FullyQualifiedName~TransactionControllerTests"
```

### Rodar apenas um método específico

```bash
dotnet test --filter "FullyQualifiedName~LedgerActivityServiceTests.CreateAsync_ValidDto_MapsToEntityAndReturnsDto"
```

### Rodar com relatório de cobertura (opcional)

```bash
dotnet test --collect:"XPlat Code Coverage"
```

---

## Resumo dos testes

### Controller — `TransactionControllerTests`

| Teste | O que valida |
|---|---|
| `GetAll_WhenCalled_ReturnsOkWithListOfTransactions` | O `GetAll` retorna `200 OK` com a lista exata devolvida pelo service; o service é chamado uma única vez |
| `GetAll_WhenNoTransactionsExist_ReturnsOkWithEmptyList` | O `GetAll` continua retornando `200 OK` (e não erro) quando o service devolve uma lista vazia |
| `Post_ValidDto_ReturnsOkWithCreatedTransaction` | O `Post` retorna `201 Created` com o DTO criado no corpo da resposta, com a action/rota corretas; o service é chamado uma única vez com os argumentos corretos |

**Ponto em aberto:** ainda não há teste para o comportamento de `Post`/`GetAll` quando o service lança uma exceção — depende de como as exceções são tratadas (no próprio controller ou via middleware).

### DTO — `CreateLedgerActivityDTOTests`

| Teste | O que valida |
|---|---|
| `CreateLedgerActivityDTO_ValueZeroOrNegative_ThrowsArgumentOutOfRangeException` (theory: `0.00`, `-0.01`, `-100.50`) | Definir `value` igual ou menor que zero lança `ArgumentOutOfRangeException` com a mensagem esperada |

### Service — `LedgerActivityServiceTests`

| Teste | O que valida |
|---|---|
| `CreateAsync_ValidDto_MapsToEntityAndReturnsDto` | O mapeamento DTO → `LedgerActivity` → `LedgerActivityDTO` está correto de ponta a ponta; o repository é chamado uma vez com a entidade mapeada corretamente |
| `CreateAsync_PassesCancellationTokenToRepository` | O `CancellationToken` recebido em `CreateAsync` é repassado ao repository, e não substituído por `default` |
| `ReadAllAsync_WhenRepositoryReturnsItems_MapsAllToDtos` | Todas as entidades retornadas pelo repository são corretamente convertidas em DTOs, preservando ordem e valores |
| `ReadAllAsync_WhenRepositoryReturnsEmpty_ReturnsEmptyList` | Retorna uma lista vazia (e não `null`) quando o repository não tem nada a retornar |

### Repository — `LedgerActivityRepositoryTests`

| Teste | O que valida |
|---|---|
| `CreateAsync_PersistsEntity_AndReturnsIt` | O `CreateAsync` realmente grava a entidade no banco (verificado por uma leitura independente), e não apenas retorna o objeto em memória |
| `CreateAsync_CalledTwice_PersistsBothEntities` | Múltiplas criações não se sobrescrevem — evita bugs de gravação única/upsert acidental |
| `ReadAllAsync_WhenEntitiesExist_ReturnsAllEntities` | Todas as entidades persistidas são retornadas com os valores corretos |
| `ReadAllAsync_WhenNoEntitiesExist_ReturnsEmptyList` | Retorna vazio (e não `null`) quando a tabela está vazia |
| `ReadAllAsync_ReturnsUntrackedEntities` | Confirma que o `.AsNoTracking()` está de fato sendo aplicado — protege contra uma regressão silenciosa de performance caso alguém remova essa chamada futuramente |

---

## Estrutura geral de cobertura

```
Controller  → 3 testes   (mapeamento HTTP, status codes)
DTO         → 1 testes   (validação de entrada)
Service     → 4 testes   (mapeamento + delegação ao repository + propagação de CancellationToken)
Repository  → 5 testes   (persistência + comportamento de queries + especificidades do EF Core)
─────────────────────────
Total: ~13 testes
```

A suíte segue bem o modelo de pirâmide de testes: a maioria são testes unitários rápidos (Controller/DTO/Model/Service), com a camada de Repository sendo a única mais "pesada" (usa um contexto EF Core InMemory).