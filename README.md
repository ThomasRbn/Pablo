# Pablo

**A free and open-source Platform as a Service.**

Deploy, run, and scale your apps without lock-in. Pablo is built in the open — no paid tiers, no proprietary walls, no surprise limits.

## Why Pablo?

- **Fully free** — use it, self-host it, fork it. No commercial license required.
- **Open source** — transparent by design; contribute and shape the roadmap.
- **Yours to run** — own your infrastructure and your data.

## Stack

| Layer | Tech |
|-------|------|
| API | .NET (Clean Architecture) |
| Web | React + Vite + Astryx |
| Database | PostgreSQL |

```
pablo/          Backend (.NET)
pablo-web/      Frontend (React)
docker/         Local Postgres
```

## Quick start

```bash
# Install git hooks (once)
make install-hooks

# Start Postgres
make db-up

# Backend
make restore && make run

# Frontend (separate terminal)
cd pablo-web && bun install && bun run dev
```

| Command | What it does |
|---------|----------------|
| `make build` | Build the .NET solution |
| `make watch` | Run API with hot reload |
| `make test` | Run backend tests |
| `make format` | Format backend + frontend |
| `make db-up` / `make db-down` | Start / stop Postgres |

## Status

Pablo is early and under active development. Star the repo and watch for releases.

## License

Free and open source. See the repository license for details.
