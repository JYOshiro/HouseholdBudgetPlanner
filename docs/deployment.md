# Deployment

<p class="page-intro">Configuration requirements, environment setup, step-by-step deployment guidance, and a pre-release checklist for the backend, frontend, and database.</p>

**Quick links:**
- [Runtime Model](#runtime-model)
- [Configuration](#configuration)
- [Deploying the Backend](#deploying-the-backend)
- [Deploying the Frontend](#deploying-the-frontend)
- [Pre-Release Checklist](#pre-release-checklist)

## Runtime Model

The platform has three independently deployable components:

| Component | Runtime | Typical Host |
|---|---|---|
| Frontend | Static files | CDN / static host (Vercel, Netlify, GitHub Pages) |
| Backend API | ASP.NET Core 9 process | App service (Render, Railway, Azure App Service) |
| Database | PostgreSQL | Managed DB (Supabase, Railway, Neon, AWS RDS) |

The frontend and backend only connect via the API base URL — they can be hosted on different services or domains.

## Configuration

### Backend — Environment Variables

Set these in your hosting environment or in `appsettings.json` / `appsettings.Production.json`:

| Variable | Description | Example |
|---|---|---|
| `ConnectionStrings__DefaultConnection` | PostgreSQL connection string | `Host=db.host;Database=budget_prod;Username=app;Password=secret` |
| `JwtSettings__Secret` | JWT signing secret — minimum 32 characters, random | `your-strong-random-32-char-secret` |
| `JwtSettings__ExpirationMinutes` | Token lifetime in minutes | `60` (production) / `1440` (dev) |
| `AllowedOrigins` | Comma-separated list of allowed frontend origins | `https://your-frontend.com` |

<div class="callout-warning">
<strong>Security:</strong> Never commit secrets to version control. Use your hosting platform's secret store, environment variables, or a secrets manager. Do not hard-code credentials in <code>appsettings.json</code>.
</div>

### Frontend — Build-Time Variables

| Variable | Description | Example |
|---|---|---|
| `VITE_API_BASE_URL` | Base URL of the backend API | `https://your-api.com/api` |

Set this in `frontend/.env.production` or as an environment variable in your build pipeline.

---

## Deploying the Backend

### 1. Set environment variables

Configure required variables in your hosting environment before deployment.

### 2. Apply database migrations

Run migrations against the target database before starting the application:

```bash
cd backend
dotnet ef database update --connection "Host=...;Database=...;Username=...;Password=..."
```

### 3. Publish

```bash
cd backend
dotnet publish -c Release -o ./publish
```

### 4. Start the service

```bash
cd backend/publish
dotnet HouseholdBudgetApi.dll
```

For managed hosting platforms (Render, Railway, etc.), point the start command to `dotnet HouseholdBudgetApi.dll` and set the environment variables through the platform UI.

---

## Deploying the Frontend

### 1. Set the API URL

Create `frontend/.env.production`:

```
VITE_API_BASE_URL=https://your-api-domain.com/api
```

### 2. Install and build

```bash
cd frontend
npm install
npm run build
```

Build output is in `frontend/dist/`. Upload this folder to your static host.

### 3. SPA routing

Configure your static host to serve `index.html` for all routes. This is required for React Router's client-side navigation to work correctly on page refresh.

- **Netlify:** add a `_redirects` file: `/* /index.html 200`
- **Vercel:** add a `vercel.json` with rewrite rules
- **Nginx:** add `try_files $uri /index.html` to your server block

---

## Pre-Release Checklist

<div class="callout">
Complete all of these before releasing to a production environment.
</div>

**Security**
- [ ] JWT secret is a strong, randomly generated value (32+ characters)
- [ ] Database credentials are stored in environment variables, not in committed config files
- [ ] CORS is restricted to the production frontend domain only
- [ ] HTTPS is enforced at the hosting or reverse proxy layer
- [ ] Swagger UI is disabled or access-restricted in production

**Database**
- [ ] Migrations applied and verified against the production database
- [ ] Default categories seeded on first startup
- [ ] Database backup procedure is defined

**Backend**
- [ ] Application starts cleanly with `ASPNETCORE_ENVIRONMENT=Production`
- [ ] Logging level is appropriate for production (no debug/verbose in production logs)
- [ ] Exception middleware returns safe error responses (no stack traces)

**Frontend**
- [ ] `VITE_API_BASE_URL` points to the production API
- [ ] Production build completes without errors or warnings
- [ ] SPA routing configured on the static host
- [ ] Auth flow tested against production API

**Operational**
- [ ] Health check or liveness endpoint implemented
- [ ] Rollback plan documented for API and schema changes
