# Deployment

## Deployment Posture
The platform is currently positioned for controlled release and environment hardening, with clear separation between frontend distribution, API hosting, and persistent data services.

## Runtime Model
- Frontend is deployable as static assets.
- Backend is deployable as ASP.NET Core API service.
- PostgreSQL persists application data.

## Configuration Requirements
- Connection string for target PostgreSQL instance.
- JWT settings with production-strength secret.
- Allowed CORS origins aligned to deployed frontend domains.
- Environment variables or secure secret store for sensitive values.

## Release Considerations
- Validate EF migrations before release.
- Ensure startup migration behavior matches operations policy.
- Confirm logging and error handling levels for production.
- Test authentication and core financial workflows post-deploy.

## Operational Follow-Up
- Implement health checks and monitoring dashboards.
- Define backup and restore procedures for PostgreSQL.
- Formalize rollback approach for API and schema changes.
