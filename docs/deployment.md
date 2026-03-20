---
title: Deployment
---

Complete strategy, configuration steps, and verification checklist for deploying to staging or production. Follow the order exactly to avoid unexpected failures.

## Strategic Overview

The app is three independently deployable services. Deploy in this order:

1. **Database** → persistent storage ready
2. **Backend** → API running, can test with Swagger
3. **Frontend** → consuming the deployed backend

Deploying out of order causes confusing failures. For example: deploying the frontend before the backend causes all API calls to fail silently.

| Service | Time | What it needs | Verify with |
|---|---|---|---|
| PostgreSQL | 10–15 min | None (greenfield) | Connection string works |
| ASP.NET Core API | 10–20 min | Database ready, JWT secret | Swagger UI responds, endpoints work |
| React frontend | 5–10 min | Backend URL, build complete | App loads, can log in |

## What You'll Need

Before starting, ensure your deployment environment has:

| Requirement | Why | Check with |
|---|---|---|
| **.NET 9 runtime** | Runs the backend service | `dotnet --version` |
| **Node.js 18+** | Builds the React frontend | `node --version` |
| **PostgreSQL 14+** | Database persistence | `psql --version` or check managed DB |
| **HTTPS (TLS cert)** | Secure auth tokens and API calls | Test with curl to your domain |
| **Environment variable support** | Secrets management for `ConnectionStrings`, `JwtSettings` | Check your hosting provider docs |

Most modern hosting (Render, Railway, Heroku, AWS, Azure) has these pre-installed. If deploying to a custom server, install them first.

## Backend Configuration

The backend reads settings from `appsettings.json`, `appsettings.Production.json`, and environment variables. **Never embed secrets in code.** Always use environment variables or your hosting platform's secret manager.

**Required settings (don't skip these):**

| Setting | Purpose | Generate as |
|---|---|---|
| `ConnectionStrings__DefaultConnection` | PostgreSQL connection | `Host=db.example.com;Port=5432;Database=budget;Username=app;Password=<strong-random>` |
| `JwtSettings__Secret` | Token signing key | 32+ random characters (e.g., `openssl rand -base64 32`) |
| `JwtSettings__Issuer` | Token issuer claim | `HouseholdBudgetApi` (literal) |
| `JwtSettings__Audience` | Token audience claim | `HouseholdBudgetApp` (literal) |
| `JwtSettings__ExpirationMinutes` | Token lifetime | `60` (adjust based on your policy) |
| `Cors__AllowedOrigins__0` | Frontend origin (production) | `https://your-app.example.com` |

### Security Rules

- ✓ **DO** use your platform's environment variable or secret manager (Render, Railway, AWS Secrets Manager, etc.)
- ✓ **DO** generate a random, 32+ character JWT secret (not guessable)
- ✓ **DO** use HTTPS for all traffic (http://localhost is the only exception)
- ✓ **DO** restrict CORS to trusted frontend origins only
- ✗ **DON'T** commit any secrets to git
- ✗ **DON'T** hardcode secret values in appsettings.Production.json
- ✗ **DON'T** reuse the same secret across environments

### Configuration Pattern (Managed Hosting)

For Render, Railway, Heroku, or similar:

1. Open your app's environment settings dashboard
2. Add each secret as an environment variable
3. Platform automatically injects them at runtime
4. Example: set `JwtSettings__Secret` → backend reads it automatically

## Frontend Configuration

The frontend reads one build-time variable:

| Variable | Purpose | Example |
|---|---|---|
| `VITE_API_URL` | Backend API base URL | `https://api.example.com/api` |

**Set before building:**

```bash
# Local development
export VITE_API_URL=http://localhost:5000/api
npm run dev

# Production build
export VITE_API_URL=https://api.example.com/api
npm run build
```

Or create a `.env.production` file:

```env
VITE_API_URL=https://api.example.com/api
```

## Deployment Flow

### Step 1: Prepare the Database

1. Create a PostgreSQL database instance (managed DB or self-hosted)
2. Create an application user with full permissions on the database
3. Test the connection string: `psql "Host=... User=... Password=... Database=..."`

Outcome: once this is complete, the backend can apply migrations and start cleanly.

### Step 2: Configure Backend Secrets

Set these in your hosting environment:

- `ConnectionStrings__DefaultConnection`
- `JwtSettings__Secret`
- `JwtSettings__Issuer`
- `JwtSettings__Audience`
- `JwtSettings__ExpirationMinutes`
- `Cors__AllowedOrigins__0` (and `_1`, `_2` if multiple origins)

Outcome: the backend has everything required to issue tokens and connect to the database.

### Step 3: Apply Database Migrations

```bash
cd backend

# Option A: With connection string argument
dotnet ef database update --connection "Host=...;Port=5432;Database=budget;Username=app;Password=secret"

# Option B: Using environment variable
export ConnectionStrings__DefaultConnection="Host=...;Port=5432;Database=budget;Username=app;Password=secret"
dotnet ef database update
```

This creates tables and seeds default categories. Wait for `Done.` message before proceeding.

### Step 4: Build and Deploy Backend

```bash
cd backend
dotnet publish -c Release -o ./publish
```

Upload `publish/` folder to your hosting service, or use your platform's git-to-deployment feature.

**Start command for your hosting platform:**

```
dotnet HouseholdBudgetApi.dll
```

**Environment variables:** Set the secrets from Step 2 in your hosting environment.

Outcome: API is deployed and ready for verification.

### Step 5: Verify Backend Before Release

Before deploying the frontend, confirm the backend works:

1. Check startup logs: `dotnet HouseholdBudgetApi.dll` should start cleanly
2. Verify database migrations completed
3. Test Swagger UI: `https://your-api.example.com/swagger` (if public)
4. Test auth: Call `POST /api/auth/register` and `POST /api/auth/login`
5. Verify token use: Call `GET /api/auth/me` with the token

If anything fails, stop here and fix it before deploying the frontend.

### Step 6: Build Frontend

```bash
cd frontend

# Set API URL for production
export VITE_API_URL=https://your-api.example.com/api

npm install
npm run build
```

Output is in `frontend/dist/`. This is static output, so no runtime process is required.

### Step 7: Deploy Frontend

Upload `dist/` to your static host:

- **GitHub Pages:** Push to `docs/` folder in your repo and enable Pages
- **Netlify:** Connect your git repo, set build command to `npm run build`, output to `dist/`
- **Vercel:** Same as Netlify, handles it automatically
- **AWS S3 + CloudFront:** Upload `dist/` contents to S3, enable CloudFront distribution
- **Custom server (Nginx):** Copy `dist/` contents to your web root

### Step 8: Configure SPA Routing Fallback

React Router requires this: all unmapped paths should serve `index.html`.

**Netlify:**
Create `frontend/_redirects`:
```
/* /index.html 200
```

**Vercel:**
Create `frontend/vercel.json`:
```json
{
  "rewrites": [
    { "source": "/(.*)", "destination": "/index.html" }
  ]
}
```

**Nginx:**
```nginx
location / {
  try_files $uri /index.html;
}
```

**GitHub Pages:**
Use the `.nojekyll` file to prevent Jekyll processing, or use a custom 404→index.html redirect.

## Release Verification Checklist

Run through this before announcing the deployment:

### Security ✓

- [ ] Production JWT secret is strong (32+ random characters)
- [ ] Backend only allows trusted frontend origins in CORS settings
- [ ] HTTPS is enabled for all traffic (frontend and backend)
- [ ] Swagger is intentionally public (or restricted)

### Backend ✓

- [ ] Database migrations applied successfully
- [ ] Startup logging shows no errors
- [ ] Default categories seeded on first run
- [ ] Exception middleware is working (test with invalid token)

### Frontend ✓

- [ ] `VITE_API_URL` points to the deployed backend
- [ ] Production build completes without errors
- [ ] SPA routing fallback is configured
- [ ] Pages load (test landing, login, dashboard)

### Integration ✓

- [ ] Can register a new account
- [ ] Can log in with created account
- [ ] Can create expenses, budgets, bills
- [ ] Dashboard loads actual month data
- [ ] Page refresh preserves authentication (token restored)

### Operational ✓

- [ ] You have a rollback plan if something breaks
- [ ] Database backups are enabled
- [ ] Environment configuration is documented (outside git)

## Common Issues

| Problem | Cause | Fix |
|---|---|---|
| Frontend can't reach backend | CORS not configured, wrong API URL | Set `VITE_API_URL` correctly, add frontend origin to backend CORS |
| `401 Unauthorized` on every request | JWT secret doesn't match, token expired | Verify secret is same on all instances, refresh token |
| Page refresh logs out the user | Token not in localStorage | Check browser storage, verify auth context recovery |
| Migrations fail at startup | Wrong connection string, user permissions | Test connection string manually, grant permissions |
| Swagger UI is blank | ASPNETCORE_ENVIRONMENT not set to Development | Set `ASPNETCORE_ENVIRONMENT=Development` for local Swagger access |

## Next Steps

If this is your first deployment, do one full dry run in staging before announcing production.

1. Set target hosting platforms for frontend and backend
2. Create environment variable documentation (for your team)
3. Test deploy cycle locally first (docker or local .NET + postgres)
4. Run through the release checklist

## Related Pages

- [Getting Started](./getting-started.html)
- [Architecture](./architecture.html)
- [API Reference](./api-reference.html)
- [Roadmap](./roadmap.html)
