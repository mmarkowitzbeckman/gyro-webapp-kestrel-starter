---
applyTo: '**'
description: Coding principles for this repo, drawn from The Pragmatic Programmer.
---

# Pragmatic Programmer principles for this repo

Apply these when writing or reviewing code. They are practical rules, ordered by
how often they come up here.

## DRY — Don't Repeat Yourself

Every piece of knowledge has one authoritative source. Don't duplicate field
lists, queries, validation, or constants.

- Field/column lists live in one place (e.g. `PROFILE_COLUMNS` / `USER_COLUMNS`
  in `src/api/database/users.js`, the `userFields` selection in
  `src/web/client/src/api.js`). Derive, don't retype.
- If you copy-paste more than a couple of lines, extract a helper or constant.

## Orthogonality — keep modules independent

Unrelated things should stay unrelated; a change in one area shouldn't ripple.

- Keep UI features self-contained (component + styles + API calls + tests
  together) so a feature can be removed by deleting its files and unwiring one
  entry point.
- Database layer, GraphQL layer, and client each own their concern; cross them
  only through their defined interfaces (db functions, resolvers, `graphqlRequest`).

## Tracer bullets — build end-to-end, thin first

When adding a feature, wire it through every layer (schema → db → GraphQL →
client → test) in one pass rather than finishing one layer in isolation.

## Design by contract & crash early

Validate at boundaries and fail loudly on broken assumptions.

- Enforce invariants at the resolver/db edge (e.g. reject duplicate usernames,
  throw `PasswordReusedError`, reject disabled logins).
- Don't add defensive checks for states that can't occur — validate only at
  real system boundaries (input, DB, network).

## Don't live with broken windows

Fix small problems as you see them; don't let quality erode.

- Add a Jest test whenever you manually confirm something works (curl, build,
  manual run) — server tests under `__tests__/`, client tests with
  `@testing-library/react`.
- Keep naming, formatting, and conventions consistent with surrounding code
  (named exports, `createId()` for PKs, `created_at`/`updated_at` on mutable
  tables).

## Make it easy to change (reversibility)

Prefer decisions that are cheap to undo. Introduce columns and fields as
optional/nullable first; migrate deployed schemas additively (see the migration
note in `src/api/database/schema.js`).

## Say what you mean, comment why not what

A comment states what the code can't show on its own, in one line. Don't restate
the next line or narrate the change.
