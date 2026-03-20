---
title: Deployment
---

This page covers the practical deployment model for the frontend, backend, and database. It focuses on what needs to be configured, what order to deploy in, and what to verify before release.

## Quick Links

- [Deployment Model](#deployment-model)
- [Environment Requirements](#environment-requirements)
- [Backend Configuration](#backend-configuration)
- [Frontend Configuration](#frontend-configuration)
- [Deployment Flow](#deployment-flow)
- [Release Checklist](#release-checklist)

## Deployment Model

The app is made of three deployable parts.

| Part | What it runs | Typical hosting options |
|---|---|---|
| Frontend | Static assets built by Vite | GitHub Pages, Netlify, Vercel, static CDN |
| Backend API | ASP.NET Core application | Azure App Service, Render, Railway, VM, container host |
| Database | PostgreSQL | Managed PostgreSQL provider or self-hosted instance |

The frontend only needs to know the backend API URL. The backend needs database access, JWT settings, and allowed frontend origins.

## Environment Requirements

| Requirement | Notes |
|---|---|
| .NET 9 runtime or SDK | Required to run the backend |
| Node.js | Required to build the frontend |
| PostgreSQL | Required for persistence |
| Environment variable support | Needed for secrets and deployment-specific config |
| HTTPS | Strongly recommended for any public environment |

## Backend Configuration

The backend reads configuration from `appsettings` and environment variables.

### Required backend settings

| Setting | Purpose | Example |
|---|---|---|
| `ConnectionStrings__DefaultConnection` | PostgreSQL connection string | `Host=db;Port=5432;Database=budget_prod;Username=app;Password=secret` |
| `JwtSettings__Secret` | JWT signing secret | `long-random-secret-at-least-32-characters` |
| `JwtSettings__Issuer` | Token issuer | `HouseholdBudgetApi` |
| `JwtSettings__Audience` | Token audience | `HouseholdBudgetApp` |
| `JwtSettings__ExpirationMinutes` | Token lifetime | `60` |
| `Cors__AllowedOrigins__0` | First allowed frontend origin | `https://your-frontend.example.com` |

> Security note: do not keep production secrets in committed configuration files. Use your host's secret or environment variable mechanism.

## Frontend Configuration

The frontend currently reads this build-time variable:

| Setting | Purpose | Example |
|---|---|---|
| `VITE_API_URL` | Base URL for API requests | `https://your-api.example.com/api` |

Example production environment file:

```env
VITE_API_URL=https://your-api.example.com/api
```

## Deployment Flow

The safest deployment order is database, backend, then frontend.

### 1. Prepare the database

- create the production PostgreSQL database
- create the application user and permissions
- confirm the connection string works from the backend host

### 2. Configure backend settings

Set the database connection string, JWT settings, and CORS allowed origins in the hosting environment.

### 3. Apply backend migrations

```bash
cd backend
dotnet ef database update --connection "Host=...;Port=5432;Database=...;Username=...;Password=..."
```

### 4. Publish and deploy the backend

```bash
cd backend
dotnet publish -c Release -o ./publish
```

Run the published app with:

```bash
cd backend/publish
dotnet HouseholdBudgetApi.dll
```

### 5. Verify the backend before shipping the frontend

- confirm the API starts cleanly
- confirm the database migrates successfully
- confirm Swagger or an equivalent test path works in the intended environment
- confirm a real login request returns a valid token

### 6. Build and deploy the frontend

```bash
cd frontend
npm install
npm run build
```

Deploy the generated `frontend/dist/` output to your static hosting provider.

### 7. Configure SPA route fallback

React Router routes need a fallback to `index.html` on refresh.

| Host | Required rule |
|---|---|
| Netlify | `_redirects` with `/* /index.html 200` |
| Vercel | Rewrite all unmatched routes to `/index.html` |
| Nginx | `try_files $uri /index.html;` |
| GitHub Pages | Use the app with a GitHub Pages-friendly SPA routing strategy or 404 redirect pattern |

## Minimal Release Verification

After deployment, verify these flows in order:

1. API health and startup logs look normal.
2. Registration works.
3. Login returns a token.
4. A protected endpoint works with that token.
5. The frontend loads and can authenticate against the deployed API.
6. A page refresh on a protected route still resolves correctly.

## Release Checklist

### Security

- [ ] production JWT secret is strong and not stored in source control
- [ ] backend only allows trusted frontend origins
- [ ] HTTPS is enabled for public traffic
- [ ] Swagger exposure is a deliberate choice for production

### Data and backend

- [ ] migrations have been applied successfully
- [ ] startup seeding completed as expected
- [ ] production logging is enabled at a sensible level
- [ ] exception responses do not expose internal details

### Frontend

- [ ] `VITE_API_URL` points to the deployed API
- [ ] production build completes successfully
- [ ] SPA route fallback is configured
- [ ] login, protected navigation, and logout have been tested

### Operations

- [ ] rollback plan exists for backend and schema changes
- [ ] backups or recovery procedures exist for the database
- [ ] environment configuration is documented outside source control

## Related Pages

- [Getting Started](./getting-started.html)
- [Frontend Guide](./frontend-guide.html)
- [API Reference](./api-reference.html)
- [Roadmap](./roadmap.html)
