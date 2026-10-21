---
applyTo: '**'
description: Git workflow rules for this repo.
---

# Git workflow

## Committing (ABSOLUTE RULE)

Never run `git commit` (or any command that creates a commit) until BOTH of
these have happened:

1. A **complete explanation of the changes** has been presented to the user —
   what changed, in which files, and why.
2. The user has **explicitly confirmed** they want it committed.

No exceptions. Do not commit proactively, do not bundle a commit into another
step, and do not assume confirmation from silence or from an earlier approval.
Staging (`git add`) is fine to prepare, but the commit itself always waits for
an explicit go-ahead after review.

Never `git push` or amend published history without a separate explicit request.
