---
applyTo: '**/*.css'
description: CSS / CSS Modules style conventions for this repo.
---

# CSS style

Follow these when writing or editing `.css` files. Prettier enforces the
mechanical formatting; the rest are conventions to keep by hand.

## Modules vs. global CSS

- Use **CSS Modules** (`*.module.css`) for component styles; import them as
  `styles` and reference classes as `styles.className`.
- Reserve plain, non-module CSS for global resets only (e.g. `index.css`).
  Don't put component-specific rules in global stylesheets.

## Class naming

- `camelCase` class names in modules (they map to `styles.camelCase`).
- Name classes by role, not appearance (`disabledBadge`, `userField`), so a
  restyle doesn't invalidate the name.

## Targeting third-party classes

- Use `:global()` when you must target a class from a third-party library
  (e.g. `:global(.someLibClass)`); keep everything else locally scoped.

## Keep styles with their feature

- A component's styles live next to the component so the feature stays
  self-contained and removable at one wiring point.
- Don't reach across features — avoid depending on another component module's
  class names.

## Formatting (Prettier-enforced — don't fight it)

- Let `npm run format:fix <file>` handle indentation, spacing, and property
  ordering rather than hand-formatting.

## Values & maintainability

- Prefer a single source of truth for repeated values (shared custom
  properties / variables) over copy-pasting the same color or size.
- Comment **why**, not what — one short line only when a rule isn't
  self-explanatory (e.g. a non-obvious `z-index` or layout hack).
