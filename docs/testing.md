# Testing

## Current Posture
- Manual verification is available through Swagger and local frontend execution.
- Automated test coverage is limited in the current baseline.

## Recommended Test Strategy
- Backend integration tests:
  - authentication register/login/me flow,
  - one complete financial CRUD lifecycle,
  - household isolation negative cases.
- Frontend tests:
  - authentication bootstrap behavior,
  - protected route gating,
  - API error rendering and retry paths.

## Suggested Quality Gates
- Build verification for frontend and backend.
- Lint and static analysis checks.
- Automated tests in CI for pull requests and main branch.

## Manual Regression Checklist
- Auth flow returns valid token and user context.
- Category, expense, and income CRUD operations succeed.
- Bill pay endpoint updates bill status as expected.
- Savings goal and contribution flows preserve totals.
- Dashboard summary returns values for selected month/year.
