# Testing

This page summarizes the current test posture, the minimum release checks, and a practical testing roadmap for backend and frontend quality.

## Quick Links

- [Current Posture](#current-posture)
- [Test Strategy](#test-strategy)
- [Minimum Quality Gates](#minimum-quality-gates)
- [Manual Regression Checklist](#manual-regression-checklist)
- [Suggested Automated Test Backlog](#suggested-automated-test-backlog)

## Current Posture

| Area | Current state |
|---|---|
| Manual API testing | Available through Swagger |
| Manual frontend testing | Available through local app runtime |
| Backend automated tests | Limited |
| Frontend automated tests | Limited |
| End-to-end coverage | Not yet established |

> Current risk: core workflows are implemented, but regression protection is still mostly manual.

## Test Strategy

### Backend priority tests

1. authentication flow: register, login, current-user
2. one full CRUD lifecycle for a financial module
3. household isolation negative tests (cross-household access attempts)
4. validation error behavior for invalid payloads

### Frontend priority tests

1. auth bootstrap with stored token
2. protected route behavior when unauthenticated
3. login and logout flow behavior
4. API error and retry UI handling

## Minimum Quality Gates

Use these as a baseline gate before merges to main:

| Gate | Scope |
|---|---|
| Build | Backend and frontend both compile successfully |
| Lint and static checks | No high-severity lint/type errors |
| Automated tests | Run available backend and frontend tests |
| Manual smoke checks | Auth and one protected data flow verified |

## Manual Regression Checklist

- [ ] Auth flow returns valid token and user context
- [ ] Category, expense, and income CRUD operations succeed
- [ ] Bill pay endpoint updates bill status as expected
- [ ] Savings goal and contribution flows preserve totals
- [ ] Dashboard summary returns values for selected month and year
- [ ] Frontend login and protected route guard behavior is correct

## Suggested Automated Test Backlog

| Priority | Test area | Suggested type |
|---|---|---|
| High | Auth service and endpoints | Backend integration tests |
| High | Household data isolation | Backend integration tests |
| High | Route guards and auth context bootstrap | Frontend unit/integration tests |
| Medium | Budget and bill rule behavior | Backend unit/integration tests |
| Medium | Error-state rendering on API failures | Frontend component tests |
| Medium | Full user journey (login to dashboard updates) | End-to-end tests |

## Related Pages

- [Getting Started](./getting-started.html)
- [API Reference](./api-reference.html)
- [Security and Privacy](./security-privacy.html)
- [Roadmap](./roadmap.html)
