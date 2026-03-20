# Getting Started

<p class="page-intro">Get the project running locally in around 15 minutes. This guide covers prerequisites, backend setup, database migration, and frontend startup.</p>

## Prerequisites

| Requirement | Minimum Version | Notes |
|---|---|---|
| .NET SDK | 9.0 | [dotnet.microsoft.com/download](https://dotnet.microsoft.com/download) |
| Node.js | 18.x | Required for the frontend |
| npm | 9.x | Included with Node.js |
| PostgreSQL | 14+ | Local installation or a remote development instance |

## Step 1 — Clone the Repository

```bash
git clone https://github.com/JYOshiro/HouseholdBudgetPlanner.git
cd HouseholdBudgetPlanner
```

## Step 2 — Configure the Backend

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

<div class="callout-tip">
<strong>Note:</strong> The database does not need to exist yet — EF Core migrations will create it in the next step.
</div>

## Step 3 — Apply Database Migrations

```bash
cd backend
dotnet restore
dotnet ef database update
```

This creates the database schema and seeds the default expense categories. You should see output ending with `Done.`

## Step 4 — Start the Backend API

```bash
dotnet run
```

The API will start on `http://localhost:5000`. Open Swagger to browse and test all endpoints:

```
http://localhost:5000/swagger
```

## Step 5 — Start the Frontend

Open a second terminal:

```bash
cd frontend
npm install
npm run dev
```

Vite will report the local URL — typically `http://localhost:5173`.

## Step 6 — Verify the Setup

Use Swagger to confirm everything is working before touching the frontend:

1. Open `http://localhost:5000/swagger`
2. `POST /api/auth/register` — create a test user account
3. `POST /api/auth/login` — log in with the same credentials and copy the `token` from the response
4. Click **Authorize** in Swagger and enter `Bearer <your-token>`
5. `GET /api/categories` — should return the seeded default categories
6. `POST /api/expenses` — record a test expense
7. `GET /api/dashboard/summary?year=2026&month=3` — should return a period summary

<div class="callout">
Once the backend is verified, return to the frontend at <code>http://localhost:5173</code> to work on the UI integration.
</div>

## Troubleshooting

| Problem | Solution |
|---|---|
| `dotnet ef database update` fails | Check the connection string — confirm PostgreSQL is running and the user has `CREATE DATABASE` permission |
| Backend starts but Swagger is blank | Ensure `ASPNETCORE_ENVIRONMENT=Development` is set — Swagger is only available in Development |
| Frontend build fails | Run `npm install` again to ensure all packages are installed |
| `401 Unauthorized` in Swagger | The token has expired or was not entered with the `Bearer ` prefix |
