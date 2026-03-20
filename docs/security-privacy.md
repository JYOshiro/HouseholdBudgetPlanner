---
title: Security and Privacy
---

This page describes the current security baseline for authentication, authorization, data isolation, and operational safeguards in Household Budget Planner.

## Quick Links

- [Security Goals](#security-goals)
- [Implemented Controls](#implemented-controls)
- [Household Isolation Model](#household-isolation-model)
- [Data and Privacy Notes](#data-and-privacy-notes)
- [Gaps and Recommended Next Controls](#gaps-and-recommended-next-controls)

## Security Goals

- allow only authenticated users to access protected finance data
- ensure users can only access records in their own household
- protect credentials and JWT signing secrets
- avoid leaking internal server details through API responses

## Implemented Controls

| Control | Current implementation |
|---|---|
| Authentication | JWT bearer token on protected endpoints |
| Password storage | BCrypt hashing with work factor 12 |
| Authorization context | Household and user claims extracted from JWT |
| Data scoping | Service-layer household filtering |
| Error handling | Global exception middleware for consistent responses |
| CORS | Configured allowed origins for frontend clients |

## Household Isolation Model

Household isolation is the most important authorization rule in this project.

Authentication uses JWT bearer tokens. Household scope is derived from JWT claims. Clients must not send `householdId` in request bodies.

- each user belongs to one household
- protected services filter by household context from claims
- requests cannot switch household scope using request body fields
- cross-household access is rejected by design

## Data and Privacy Notes

| Area | Current approach |
|---|---|
| Data storage | Financial records are persisted in PostgreSQL |
| API contract safety | DTO boundaries reduce accidental entity data exposure |
| Configuration separation | Development and production settings are separated |
| Secrets | Intended to be environment-managed, not hard-coded |

Privacy scope in the current baseline:

- data is household-scoped, not globally shared
- no third-party banking integrations are active
- no external analytics pipeline is described in the current implementation docs

## Gaps and Recommended Next Controls

These controls are recommended before broad production exposure.

| Priority | Recommendation | Why it matters |
|---|---|---|
| High | Enforce HTTPS end to end | Protects credentials and tokens in transit |
| High | Keep JWT secrets in secure environment storage | Prevents accidental secret leakage |
| High | Restrict CORS to trusted frontend origins | Reduces cross-origin attack surface |
| Medium | Add rate limiting on auth endpoints | Reduces brute-force and abuse risk |
| Medium | Add audit logging for sensitive actions | Improves traceability and incident response |
| Medium | Expand integration tests for auth and household isolation | Prevents security regressions |

## Related Pages

- [Architecture](./architecture.html)
- [API Reference](./api-reference.html)
- [Deployment](./deployment.html)
- [Testing](./testing.html)
