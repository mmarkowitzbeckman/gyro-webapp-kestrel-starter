---
applyTo: '**/*.{js,jsx}'
description: JavaScript/JSX code style conventions for this repo.
---

# JavaScript / JSX code style

Follow these when writing or editing `.js` / `.jsx` files. Prettier and oxlint
enforce the mechanical parts; the rest are conventions to keep by hand.

## Modules

- Use **named exports**, never `export default` (except where a tool requires
  it, e.g. config files).
- Server code uses bare `src/...` imports (resolved via `jsconfig.json`
  `baseUrl`), not deep relative paths. Client code uses relative imports.

## Formatting (Prettier-enforced — don't fight it)

- Semicolons required.
- Double quotes for strings.
- Trailing commas on multiline literals.
- Run `npm run format:fix <file>` (client) before committing rather than
  hand-formatting.
- methods should always have at least 1 space below them before the next line of code

## Naming

- `camelCase` for variables, functions, and object keys.
- `PascalCase` for React components and classes.
- `SCREAMING_SNAKE_CASE` for module-level constants that are fixed config.
- Database columns are `snake_case`; map them to `camelCase` at the boundary —
  never leak `snake_case` keys into GraphQL/JS objects.

## Identifiers & data

- No auto-incrementing IDs — use `createId()` from `@paralleldrive/cuid2` for
  all primary keys.
- Any mutable table row carries `created_at`/`updated_at`; bump `updated_at`
  on every `UPDATE`. Append-only/audit rows get only `created_at`.

## Functions

- Prefer small, single-purpose functions with a single authoritative source
  for shared data (DRY): field/column lists live in one constant and are
  derived, not retyped.
- Validate at boundaries (resolvers, db edge) and fail loudly; don't add
  defensive checks for states that can't occur.
- Always prefer arrow function expressions
- method names should describe exactly what the method does, and if its part of a set then it should lead with that set's name:
  example: user -> userAdd, userEdit, userDelete, ect..

## React (`.jsx`)

- Function components with hooks; no class components.
- Keep each feature self-contained — component, its styles (`*.module.css`),
  API calls, and tests grouped together so it can be added/removed at one
  wiring point.
- Use CSS Modules with `camelCase` class names for component styles.

## GraphQL client strings

- Define queries/mutations as template-literal strings in
  `src/web/client/src/api.js` and send them through `graphqlRequest()`.
- Keep a single shared field selection and interpolate it into
  queries/mutations rather than duplicating field lists.

## Comments

- Comment **why**, not what. One short line stating what the code can't show
  on its own. No change-narration or restating the next line.

## Tests

- Server tests live under `__tests__/` next to the code; client tests use
  `@testing-library/react` with `fetch` mocked.
- Add a Jest test whenever you manually confirm something works.
