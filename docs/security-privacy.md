# Security and Privacy

## Security Objectives
- Ensure authenticated access to protected financial resources.
- Enforce household-level data boundaries.
- Protect credentials and token flows.

## Implemented Controls
- JWT bearer authentication for protected API routes.
- Claims-based user and household context extraction.
- BCrypt password hashing with work factor 12.
- Global exception middleware to avoid inconsistent error handling.

## Household Isolation Model
- User access is constrained to their assigned household.
- Services apply household filters before returning or mutating data.
- Cross-household data access is rejected.

## Data Handling Notes
- Financial records are persisted in PostgreSQL.
- API contracts use DTO boundaries to avoid exposing entity internals.
- Development and production settings are separated in appsettings files.

## Operational Recommendations
- Store production JWT secrets in secure secret management.
- Enforce HTTPS in production environments.
- Add rate limiting and audit logging for sensitive operations.
- Expand security-focused integration testing before broad release.
