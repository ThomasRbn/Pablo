# Pablo.API.Tests

Integration tests for `Pablo.API` using `WebApplicationFactory` (`PabloApiFactory`).

## Architecture rule

**The test project must mirror the API project layout.**

When the API adds or changes a feature under `src/Pablo.API/Features/{Name}/`, put the corresponding tests under:

```
tests/Pablo.API.Tests/Features/{Name}/
```

Use the same feature folder name, and namespaces that match folders (`Pablo.API.Tests.Features.{Name}`).

| API | Tests |
|-----|--------|
| `src/Pablo.API/Features/{Name}/` | `tests/Pablo.API.Tests/Features/{Name}/` |
| `src/Pablo.API/Infrastructure/` | Shared test helpers only if needed (prefer keeping the factory at the project root) |
| Host endpoints in `Program.cs` (e.g. `/health`) | Project root (or a small `Host/` folder) — not under `Features/` |

Do **not** invent a parallel taxonomy (e.g. flat `Controllers/`, `Services/`, or `Integration/` trees that do not exist in the API).

## Current layout

```
Pablo.API.Tests/
  PabloApiFactory.cs       # WebApplicationFactory; EF → InMemory for tests
  HealthEndpointTests.cs   # host smoke test for MapHealthChecks("/health")
  Features/                # add {Name}/ here as features appear in the API
```

## Adding a feature test

1. Create `Features/{Name}/` (same `{Name}` as in the API).
2. Add tests next to the surface you exercise (e.g. `{Name}ControllerTests.cs` for HTTP).
3. Prefer `PabloApiFactory` + `HttpClient` for endpoint coverage; keep business-rule unit tests in the same feature folder when useful.

## Running

From the repo root:

```bash
make test
```
