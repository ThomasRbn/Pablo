SOLUTION := pablo/pablo.slnx
API_PROJECT := pablo/src/Pablo.API
WEB_DIR := pablo-web
COMPOSE := docker compose -f docker/docker-compose.yml
EF := dotnet ef --project $(API_PROJECT)

.PHONY: install-hooks restore build run watch test test-backend format clean db-up db-down db-logs db-migrate db-reset

install-hooks:
	cp hooks/pre-commit .git/hooks/pre-commit
	chmod +x .git/hooks/pre-commit
	@echo "Installed pre-commit hook → .git/hooks/pre-commit"

restore:
	dotnet restore $(SOLUTION)

build:
	dotnet build $(SOLUTION)

run:
	dotnet run --project $(API_PROJECT)

watch:
	dotnet watch run --project $(API_PROJECT)

test: test-backend

test-backend:
	dotnet test $(SOLUTION)

format:
	dotnet format $(SOLUTION)
	cd $(WEB_DIR) && bunx biome check --write .

clean:
	dotnet clean $(SOLUTION)

db-up:
	$(COMPOSE) up -d

db-down:
	$(COMPOSE) down

db-logs:
	$(COMPOSE) logs -f postgres

db-migrate:
	$(EF) database update

# Drop Postgres volume, recreate, apply migrations. Root user (root/root) is seeded on next API start.
db-reset:
	$(COMPOSE) down -v
	$(COMPOSE) up -d --wait
	$(EF) database update
	@echo "Database reset. Start the API (make run) to seed the root user (username/password: root/root)."
