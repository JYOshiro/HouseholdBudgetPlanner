---
title: Getting Started
---

Get the project running locally with a working database, backend API, and frontend client. This guide is optimized for first-time setup and quick verification.

## Quick Links

- [Prerequisites](#prerequisites)
- [Step 1: Clone the Repository](#step-1-clone-the-repository)
- [Step 2: Configure the Backend](#step-2-configure-the-backend)
- [Step 3: Apply Database Migrations](#step-3-apply-database-migrations)
- [Step 4: Start the Backend API](#step-4-start-the-backend-api)
- [Step 5: Start the Frontend](#step-5-start-the-frontend)
- [Step 6: Verify the Setup](#step-6-verify-the-setup)
- [Troubleshooting](#troubleshooting)

> Recommended order: backend first, then frontend.

## Prerequisites

| Requirement | Minimum Version | Notes |
|---|---|---|
| .NET SDK | 9.0 | [dotnet.microsoft.com/download](https://dotnet.microsoft.com/download) |
| Node.js | 18.x | Required for the frontend |
| npm | 9.x | Included with Node.js |
| PostgreSQL | 14+ | Local installation or a remote development instance |

## Step 1: Clone the Repository

```bash
git clone https://github.com/JYOshiro/HouseholdBudgetPlanner.git
cd HouseholdBudgetPlanner
```

## Step 2: Configure the Backend

Open `backend/appsettings.Development.json` and set your local PostgreSQL connection string and a JWT secret:

```json
{
	"ConnectionStrings": {
		"DefaultConnection": "Host=localhost;Database=household_budget;Username=your_user;Password=your_password"
	},
	"JwtSettings": {
		"Secret": "your-development-secret-minimum-32-characters",
		"ExpirationMinutes": 1440
	}
}
```

Also confirm that CORS includes your frontend origin (typically `http://localhost:5173`).

> The database does not need to exist yet. EF Core migrations will create it.

## Step 3: Apply Database Migrations

```bash
cd backend
dotnet restore
dotnet ef database update
```

This creates the database schema and seeds the default expense categories. You should see output ending with `Done.`

## Step 4: Start the Backend API

```bash
dotnet run
```

The API should start on `http://localhost:5000`.

Open Swagger to browse and test endpoints:

```
http://localhost:5000/swagger
```

## Step 5: Start the Frontend

Open a second terminal:

```bash
cd frontend
npm install
npm run dev
```

Vite usually starts at `http://localhost:5173`.

If your frontend cannot reach the API, set this environment variable in `frontend/.env.local`:

```env
VITE_API_URL=http://localhost:5000/api
```

## Step 6: Verify the Setup

Use Swagger to confirm the setup before frontend integration work:

1. Open `http://localhost:5000/swagger`
2. Call `POST /api/auth/register` to create a test user
3. Call `POST /api/auth/login` and copy the returned `token`
4. Click **Authorize** in Swagger and enter `Bearer <your-token>`
5. Call `GET /api/categories` and confirm seeded categories are returned
6. Call `POST /api/expenses` and create a test expense
7. Call `GET /api/dashboard/summary?year=2026&month=3` and confirm summary data

> Once backend verification is complete, continue in the frontend at `http://localhost:5173`.

## Troubleshooting

| Problem | Solution |
|---|---|
| `dotnet ef database update` fails | Check the connection string — confirm PostgreSQL is running and the user has `CREATE DATABASE` permission |
| Backend starts but Swagger is blank | Ensure `ASPNETCORE_ENVIRONMENT=Development` is set — Swagger is only available in Development |
| Frontend build fails | Run `npm install` again to ensure all packages are installed |
| Frontend cannot call backend | Set `VITE_API_URL=http://localhost:5000/api` and restart Vite |
| `401 Unauthorized` in Swagger | The token has expired or was not entered with the `Bearer ` prefix |

## Related Pages

- [API Reference](./api-reference.html)
- [Frontend Guide](./frontend-guide.html)
- [Deployment](./deployment.html)
