# AGENTS.md

Project-specific guidance for AI coding agents.

<!-- SHADCN:START -->
shadcn/ui (preset `b3kK6DGsq`, primary `#FF6900`)

CLI: run every command as `bunx --bun shadcn@latest <cmd>` (shown below as `shadcn ...`).

SETUP — global styles live in `src/styles.css` (Tailwind v4 + CSS variables). Wrap overlays that need tooltips with `TooltipProvider` in the root route.

WORKFLOW — discover, don't guess. Before writing UI:
1. `shadcn search @shadcn -q "<query>"` — find components in the registry.
2. `shadcn add <component>` — add source under `src/components/ui/`.
3. `shadcn docs <component>` — props and usage links for what you add.

RULES:
- Prefer existing `src/components/ui/*` primitives before hand-rolling markup.
- Semantic colors only: `bg-primary`, `text-muted-foreground`, `border-border`, etc. Brand primary is `#FF6900` via `--primary` in `src/styles.css`.
- Compose forms with `Field` / `FieldGroup` / `FieldLabel` + `Input`.
- App chrome: `SidebarProvider` + `Sidebar` + `SidebarInset` (see `_app.tsx`).
- Icons: `@phosphor-icons/react` (preset icon library).
- Use `cn()` from `@/lib/utils` for conditional classes.

MORE CLI:
  apply --preset <code>   switch theme/fonts/icons on an existing project
  info                    project config + installed components
  docs <name>             documentation links for a component
<!-- SHADCN:END -->
