# Getting Started

## Prerequisites
- .NET 8 SDK
- Node.js and npm
- PostgreSQL (local or remote development instance)

## 1. Clone and Open
1. Clone the repository.
2. Open the project workspace.

## 2. Configure Backend
1. Go to the backend folder.
2. Update development connection string in appsettings.Development.json.
3. Confirm JWT settings are present and valid for development.

## 3. Apply Database Migrations
1. Run dotnet restore.
2. Run dotnet ef database update.

## 4. Run Backend API
1. Run dotnet run from the backend folder.
2. Open Swagger at /swagger in the reported backend URL.

## 5. Run Frontend
1. Go to the frontend folder.
2. Run npm install.
3. Run npm run dev.
4. Open the local URL reported by Vite.

## 6. Verify Baseline
- Register a user account.
- Log in and obtain bearer token.
- Exercise at least one CRUD flow in Swagger (for example categories or expenses).
- Verify dashboard summary endpoint returns data for a selected period.
