SOLUTION := pablo/pablo.slnx
API_PROJECT := pablo/src/Pablo.API
WEB_DIR := pablo-web

.PHONY: install-hooks restore build run watch test format clean

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

test:
	dotnet test $(SOLUTION)

format:
	dotnet format $(SOLUTION)
	cd $(WEB_DIR) && bunx biome check --write .

clean:
	dotnet clean $(SOLUTION)
