# How to write tests in Pablo.API.Tests

Quick reference for adding tests and choosing xUnit attributes.

Also see [README.md](README.md) for folder layout (must mirror the API).

## Create a test file

1. **Place it correctly**
   - Feature under test → `Features/{Name}/YourThingTests.cs`
   - Host-level (e.g. `/health`) → project root (or `Host/`)
2. **Namespace matches folder** — e.g. `Pablo.API.Tests.Features.Auth`
3. **Name the class** `…Tests` and methods like `Verb_scenario_expectedResult`
4. **HTTP / integration** — inject `PabloApiFactory` via `IClassFixture<PabloApiFactory>`
5. **Run** — `make test` from the repo root

### Minimal HTTP test

```csharp
using System.Net;

namespace Pablo.API.Tests.Features.Example;

public class ExampleControllerTests(PabloApiFactory factory) : IClassFixture<PabloApiFactory>
{
    [Fact]
    public async Task Get_example_returns_ok()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync("/api/example");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
```

`PabloApiFactory` boots the real API with EF InMemory — no Postgres required.

### Pure unit test (no HTTP)

Same feature folder, no factory. Construct the service (or use mocks) and assert on return values. Prefer this for pure business rules; prefer the factory when you care about routing, status codes, or DI wiring.

---

## Test types (xUnit)

xUnit discovers public methods marked with attributes. The two you will use most: **`[Fact]`** and **`[Theory]`**.

### `[Fact]` — one scenario, one run

A single, fixed case. No parameters (or only values you hard-code inside the method).

**Use when:** one clear behaviour (happy path, one error case, one status code).

```csharp
[Fact]
public async Task Get_health_returns_healthy()
{
    var client = factory.CreateClient();
    var response = await client.GetAsync("/health");
    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
}
```

### `[Theory]` — same test, many inputs

One method, multiple data rows. xUnit runs the method **once per row**. Failures show which row failed.

**Use when:** the logic is the same and only inputs/expected outputs change (validation matrix, status codes per payload, edge values).

You must supply data with one of:

| Attribute | When |
|-----------|------|
| `[InlineData(...)]` | Small, literal values on the method |
| `[MemberData(nameof(…))]` | Rows from a static property/method (objects, complex cases) |
| `[ClassData(typeof(…))]` | Rows from a dedicated class implementing `IEnumerable<object[]>` |

#### `InlineData` (most common)

```csharp
[Theory]
[InlineData("", false)]
[InlineData("a@b.com", true)]
[InlineData("not-an-email", false)]
public void IsValidEmail_matches_expectation(string email, bool expected)
{
    Assert.Equal(expected, EmailRules.IsValid(email));
}
```

Parameter types must match what `InlineData` can express (primitives, strings, enums, `null`). Not good for rich objects.

#### `MemberData` (richer rows)

```csharp
public static TheoryData<string, HttpStatusCode> CreateUserCases => new()
{
    { """{"email":"a@b.com"}""", HttpStatusCode.Created },
    { """{"email":""}""", HttpStatusCode.BadRequest },
};

[Theory]
[MemberData(nameof(CreateUserCases))]
public async Task Post_user_returns_expected_status(string json, HttpStatusCode expected)
{
    var client = factory.CreateClient();
    var response = await client.PostAsync(
        "/api/users",
        new StringContent(json, Encoding.UTF8, "application/json"));

    Assert.Equal(expected, response.StatusCode);
}
```

`TheoryData<…>` keeps rows typed; `IEnumerable<object[]>` also works but is easier to get wrong.

### Fact vs Theory — rule of thumb

| Choose | If… |
|--------|-----|
| **Fact** | One story; copying the method would be clearer than a table |
| **Theory** | Three or more similar cases, or a clear input → output matrix |

Avoid Theories with a single `InlineData` — use a Fact. Avoid huge Theories that hide unrelated behaviours — split Facts/Theories by behaviour.

---

## Other useful attributes

| Attribute | Meaning |
|-----------|---------|
| `[Fact(Skip = "reason")]` / `[Theory(Skip = "…")]` | Temporarily exclude; shows as skipped in results |
| `[Trait("Category", "Integration")]` | Optional labels for filtering (`dotnet test --filter Category=Integration`) |

We do not require Traits today; add them only if you need to filter suites.

---

## Fixtures (sharing setup)

| Pattern | Lifetime | Use here |
|---------|----------|----------|
| **Constructor injection + `IClassFixture<T>`** | One `T` per test **class** | `PabloApiFactory` — share one test server across methods in the class |
| **`ICollectionFixture<T>`** | One `T` per collection of classes | Rare; only if several classes must share heavy setup |
| **Constructor only (no fixture)** | New instance every test method | Pure unit tests with cheap setup |

```csharp
public class HealthEndpointTests(PabloApiFactory factory) : IClassFixture<PabloApiFactory>
```

Primary constructor + `IClassFixture<PabloApiFactory>` is the standard pattern for HTTP tests in this project.

---

## Assertions

Built-in `Assert.*` is enough for now (`Equal`, `True`, `NotNull`, `Throws`, `Contains`, …). Prefer asserting **observable outcomes** (status code, response body, DB state via a scoped `PabloDbContext` from `factory.Services`) over implementation details.

---

## Checklist

- [ ] File lives under `Features/{Name}/` (or host root) — see [README.md](README.md)
- [ ] Namespace matches folder
- [ ] `[Fact]` for a single case; `[Theory]` + data for a matrix
- [ ] HTTP tests use `IClassFixture<PabloApiFactory>`
- [ ] `make test` passes
