# Remp.Tests

Unit tests for the RECAM service layer (`Remp.Service`).

The tests run entirely in memory. They do **not** need SQL Server, MongoDB, Azure Blob Storage or Azurite, so they behave the same on any machine and in CI.

## Tools

| Tool | Purpose |
|---|---|
| [xUnit](https://xunit.net/) | Test framework (`[Fact]`, `Assert`) |
| [Moq](https://github.com/devlooped/moq) | Creates mock implementations of interfaces |
| [FluentAssertions](https://fluentassertions.com/) | Readable assertions (`result.Should().Be(1)`) |

## Running the tests

From the solution root:

```bash
dotnet test
```

To collect code coverage (the `coverlet.collector` package is included by the xUnit template):

```bash
dotnet test --collect:"XPlat Code Coverage"
```

## Project structure

```
Remp.Tests/
├── TestHelpers/
│   ├── MockHelper.cs            # Shared mocks and test dependencies
│   └── TestData.cs              # Builders for listing cases, agents and media assets
├── ListingCaseServiceTests.cs   # Tests for ListingCaseService
└── README.md
```

## What is mocked and why

Services depend on interfaces, which are registered with dependency injection in the API. In tests there is no DI container, so each dependency is either mocked with Moq or created directly.

| Dependency | Real implementation | In tests | Reason |
|---|---|---|---|
| Repositories (`IListingCaseRepository`, `IAgentRepository`, ...) | EF Core queries against SQL Server | **Mocked** | Isolate business logic from the database |
| `IUnitOfWork` | `SaveChangesAsync` / transactions on `RempDbContext` | **Mocked** | No database writes; can simulate save failures |
| `IBlobStorageService` | Azure Blob Storage (Azurite locally) | **Mocked** | No network or storage account needed |
| `IActivityLogService` | Writes activity logs to MongoDB | **Mocked** | No MongoDB needed; used to verify logging |
| Validators (`IValidator<T>`) | FluentValidation validators | **Real** | Validation rules are part of the business logic under test |
| `IMapper` | AutoMapper with `MappingProfile` | **Real** | Mappings are tested together with the service |
| `IOptions<FrontendSettings>` | `appsettings.json` | `Options.Create(...)` | Simple value object, no mocking needed |
| Current user | JWT claims read in controllers | Plain strings (e.g. `"user-1"`) | Services receive `userId` / `role` as parameters |

### Why repositories instead of `DbContext`

The service layer never uses `RempDbContext` directly. It talks to repositories and `IUnitOfWork`. Mocking these interfaces:

- keeps tests focused on service behaviour, not EF Core internals,
- avoids the complexity of mocking `DbSet<T>` and async LINQ providers,
- lets a test control exactly what each query returns, including `null` for "not found".

Repository queries themselves are better covered by integration tests against a real database.

## MockHelper

`TestHelpers/MockHelper.cs` creates the mocks shared by most test classes:

| Method | Returns | Default behaviour |
|---|---|---|
| `CreateMapper()` | `IMapper` | Real AutoMapper configured with `MappingProfile` |
| `CreateUnitOfWork()` | `Mock<IUnitOfWork>` | `SaveChangesAsync()` returns `1` |
| `CreateBlobStorage()` | `Mock<IBlobStorageService>` | `GetReadUrl(name)` returns `https://fake-storage/{name}?sas=test`; upload and delete complete without doing anything |
| `CreateActivityLog()` | `Mock<IActivityLogService>` | All log methods complete without doing anything |

Repository mocks are created in each test class because their setup depends on the scenario.

## Writing a test

### Naming

`MethodName_WhenCondition_ExpectedResult`, for example
`CreateListingCaseAsync_WhenSaveFails_ThrowsDbUpdateException`.

### Structure

xUnit creates a new instance of the test class for every test, so the constructor builds fresh mocks and a fresh service each time. Tests never share state.

Each test follows **Arrange / Act / Assert**:

```csharp
[Fact]
public async Task CreateListingCaseAsync_WhenDtoIsValid_ReturnsNewListingCaseId()
{
    // Arrange: prepare input and configure mocks
    CreateListingCaseDto dto = CreateValidDto();
    _mockListingCaseRepository
        .Setup(m => m.Add(It.IsAny<ListingCase>()))
        .Callback<ListingCase>(listingCase => listingCase.Id = 1);

    // Act: call the method under test
    int result = await _listingCaseService.CreateListingCaseAsync(dto, "user-1", "127.0.0.1");

    // Assert: check the result and the interactions
    result.Should().Be(1);
    _mockUnitOfWork.Verify(m => m.SaveChangesAsync(), Times.Once);
}
```

### Common Moq patterns

| Goal | Code |
|---|---|
| Return data from an async query | `.Setup(r => r.GetListingCaseAsync(1)).ReturnsAsync(listingCase)` |
| Simulate "not found" | `.ReturnsAsync((ListingCase?)null)` |
| Simulate a database failure | `.Setup(u => u.SaveChangesAsync()).ThrowsAsync(new DbUpdateException(...))` |
| Run code when a `void` method is called (e.g. assign an id) | `.Setup(r => r.Add(It.IsAny<ListingCase>())).Callback<ListingCase>(l => l.Id = 1)` |
| Match any argument | `It.IsAny<T>()` |
| Match an argument by condition | `It.Is<ListingCase>(l => l.UserId == "user-1")` |
| Check a method was called | `.Verify(..., Times.Once)` |
| Check a method was never called | `.Verify(..., Times.Never)` |

### What to verify

Besides the return value, tests check the side effects that matter to the business rules:

- On success: the entity is added, `SaveChangesAsync` is called once, and the matching MongoDB activity log is written.
- On validation, permission or database failure: an exception is thrown, nothing is saved, and no success log is written.