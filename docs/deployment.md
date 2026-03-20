---
title: Deployment
---

Deployment strategy, configuration, and release checklist for the full app stack.

## Deployment Strategy

The app has three independently deployable parts. The safe order is database, backend, then frontend. This lets you verify each layer before moving to the next.

| Part | Deployment time | Complexity | Dependencies |
|---|---|---|---|
| Database | 10-15 min | Low | None (new environment) |
| Backend | 10-20 min | Medium | Database must be ready |
| Frontend | 5-10 min | Low | None (static files) |

The frontend only needs the backend URL. The backend needs database credentials and JWT settings.

## Environment Requirements

To deploy and run the app:

| Requirement | Details |
|---|---|
| **.NET 9 runtime** | Run the backend |
| **Node.js 18+** | Build the frontend |
| **PostgreSQL 14+** | Data persistence |
| **HTTPS capable** | Recommended for production |
| **Environment variable support** | Needed for secrets in your hosting platform |

## Backend Configuration

The backend reads settings from `appsettings.json`, `appsettings.Production.json`, and environment variables.

**Required settings:**

| Setting | Purpose | Example |
|---|---|---|
| `ConnectionStrings__DefaultConnection` | Database connection | `Host=db.example.com;Port=5432;Database=budget;Username=app;Password=secret` |
| `JwtSettings__Secret` | JWT signing key | `your-secret-key-32-chars-minimum-random` |
| `JwtSettings__Issuer` | Token issuer claim | `HouseholdBudgetApi` |
| `JwtSettings__Audience` | Token audience claim | `HouseholdBudgetApp` |
| `JwtSettings__ExpirationMinutes` | Token lifetime | `60` |
| `Cors__AllowedOrigins__0` | Frontend origin | `https://your-frontend.example.com` |

**Security best practices:**

- Never commit production secrets to git
- Use your hosting platform's secret manager or environment variables
- JWT secret must be 32+ random characters
- Use HTTPS for all public traffic

**Example for managed hosting (Render, Railway, etc.):**

1. Create environment variables in your hosting dashboard
2. Set them as deployment-time secrets (not in code)
3. Reference them by name at runtime

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

Nothing else to do; migrations will create tables.

### Step 2: Configure Backend Secrets

Set these in your hosting environment:

- `ConnectionStrings__DefaultConnection`
- `JwtSettings__Secret`
- `JwtSettings__Issuer`
- `JwtSettings__Audience`
- `JwtSettings__ExpirationMinutes`
- `Cors__AllowedOrigins__0` (and `_1`, `_2` if multiple origins)

### Step 3: Apply Database Migrations

```bash
cd backend

# Option A: With connection string argument
dotnet ef database update --connection "Host=...;Port=5432;Database=budget;Username=app;Password=secret"

# Option B: Using environment variable
export ConnectionStrings__DefaultConnection="Host=...;Port=5432;Database=budget;Username=app;Password=secret"
dotnet ef database update
```

This creates tables and seeds default categories. Wait for `Done.` message.

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

### Step 5: Verify Backend Before Release

Before deploying the frontend, confirm the backend works:

1. Check startup logs: `dotnet HouseholdBudgetApi.dll` should start cleanly
2. Verify database migrations completed
3. Test Swagger UI: `https://your-api.example.com/swagger` (if public)
4. Test auth: Call `POST /api/auth/register` and `POST /api/auth/login`
5. Verify token use: Call `GET /api/auth/me` with the token

If anything fails, don't proceed to the frontend.

### Step 6: Build Frontend

```bash
cd frontend

# Set API URL for production
export VITE_API_URL=https://your-api.example.com/api

npm install
npm run build
```

Output is in `frontend/dist/`. This is 100% static—no runtime required.

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
| Swagger UI is blank | ASPNETCORE_ENVIRONMENT not set to Development | Set `ASPNETCORE_ENVIRONMENT=Production` if you want Swagger restricted |

## Next Steps

1. Set target hosting platforms for frontend and backend
2. Create environment variable documentation (for your team)
3. Test deploy cycle locally first (docker or local .NET + postgres)
4. Run through the release checklist

## Related Pages

- [Getting Started](./getting-started.html)
- [Architecture](./architecture.html)
- [API Reference](./api-reference.html)
- [Roadmap](./roadmap.html)
